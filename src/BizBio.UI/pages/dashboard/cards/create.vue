<template>
  <WizardLayout
    :current-step="state.currentStep"
    :can-proceed="canProceedFromStep"
    :show-nav="state.currentStep !== 3"
    :next-label="state.currentStep === 2 ? 'Save & Publish' : 'Next'"
    @next="handleNext"
    @prev="previousStep"
    @go-to-step="goToStep"
  >
    <Step1Plan
      v-if="state.currentStep === 1"
      :selected-plan="state.selectedPlan"
      @plan-selected="handlePlanSelected"
    />

    <Step2Build
      v-else-if="state.currentStep === 2"
      :state="state"
      :max-contact-buttons="maxContactButtons"
      :max-contact-info="maxContactInfo"
      :max-social-links="maxSocialLinks"
    />

    <Step3Publish
      v-else-if="state.currentStep === 3"
      :state="state"
    />
  </WizardLayout>
</template>

<script setup lang="ts">
import WizardLayout from '~/components/cards/wizard/WizardLayout.vue'
import Step1Plan from '~/components/cards/wizard/Step1Plan.vue'
import Step2Build from '~/components/cards/wizard/Step2Build.vue'
import Step3Publish from '~/components/cards/wizard/Step3Publish.vue'

definePageMeta({
  middleware: 'auth',
  layout: false,
})

useHead({ title: 'Create Business Card' })

const {
  state,
  selectPlan,
  nextStep,
  previousStep,
  goToStep,
  canProceedFromStep,
  maxContactButtons,
  maxContactInfo,
  maxSocialLinks,
  generateSlug,
  generateQrCode,
} = useBusinessCardCreation()

const profilesApi = useProfilesApi()
const saving = ref(false)

const handlePlanSelected = (planId: string) => {
  selectPlan(planId as any)
  nextStep()
}

const handleNext = async () => {
  if (state.value.currentStep === 2) {
    await saveCard()
  } else {
    nextStep()
  }
}

const saveCard = async () => {
  if (saving.value) return
  saving.value = true

  try {
    // Generate slug if not set
    if (!state.value.slug) {
      state.value.slug = generateSlug(state.value.firstName, state.value.lastName)
    }

    const payload = {
      slug: state.value.slug,
      template: state.value.selectedTemplate,
      basicInfo: {
        firstName: state.value.firstName,
        lastName: state.value.lastName,
        headline: state.value.headline,
        company: state.value.company,
        photoUrl: state.value.photoUrl,
        bio: state.value.bio,
      },
      contactButtons: state.value.contactButtons,
      contactInfo: state.value.contactInfo,
      socialLinks: state.value.socialLinks,
      sections: state.value.sections,
      appearance: state.value.appearance,
      subscriptionId: state.value.subscriptionId,
      plan: state.value.selectedPlan,
    }

    const result = await profilesApi.create(payload) as any
    const slug = result?.slug || result?.data?.slug || state.value.slug

    state.value.slug = slug
    state.value.profileUrl = `https://bizbio.co.za/${slug}`
    state.value.qrCodeSvg = generateQrCode(state.value.profileUrl)

    nextStep()
  } catch (err) {
    console.error('Failed to save card:', err)
    // Still advance for demo / if API not ready yet
    if (!state.value.slug) {
      state.value.slug = generateSlug(state.value.firstName, state.value.lastName)
    }
    state.value.profileUrl = `https://bizbio.co.za/${state.value.slug}`
    state.value.qrCodeSvg = generateQrCode(state.value.profileUrl)
    nextStep()
  } finally {
    saving.value = false
  }
}
</script>
