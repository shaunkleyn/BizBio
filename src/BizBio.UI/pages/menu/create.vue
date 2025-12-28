<template>
  <NuxtLayout name="dashboard">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Progress Steps -->
      <div class="mb-12">
        <div class="flex items-center justify-center">
          <div v-for="step in 4" :key="step" class="flex items-center">
            <!-- Step Circle -->
            <div :class="[
              'flex items-center justify-center w-12 h-12 rounded-full font-bold transition-all',
              currentStep >= step
                ? 'btn-gradient text-white'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)]'
            , 'shadow-md-2 hover:shadow-md-4']">
              <i v-if="currentStep > step" class="fas fa-check"></i>
              <span v-else>{{ step }}</span>
            </div>

            <!-- Step Label -->
            <div class="hidden sm:block ml-3 mr-8">
              <div :class="[
                'text-sm font-semibold',
                currentStep >= step ? 'text-[var(--dark-text-color)]' : 'text-[var(--gray-text-color)]'
              ]">
                {{ getStepLabel(step) }}
              </div>
            </div>

            <!-- Connector Line -->
            <div v-if="step < 4" :class="[
              'hidden sm:block w-16 h-1 transition-all',
              currentStep > step ? 'bg-md-primary' : 'bg-[var(--light-border-color)]'
            ]"></div>
          </div>
        </div>
      </div>

      <!-- Step 1: Plan Selection -->
      <div v-if="currentStep === 1">
        <div class="text-center mb-12">
          <h1 class="text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
            Choose Your Menu Plan
          </h1>
          <p class="text-xl text-[var(--gray-text-color)] max-w-2xl mx-auto">
            Start your 14-day free trial. No credit card required.
          </p>
        </div>

        <!-- Pricing Cards -->
        <div class="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto mb-8">
          <div
            v-for="plan in menuPlans"
            :key="plan.id"
            @click="handlePlanSelect(plan)"
            :class="[
              'rounded-2xl shadow-xl p-8 transition-all duration-300 cursor-pointer',
              plan.popular
                ? 'bg-gradient-to-br from-[var(--primary-color)] to-[var(--accent3-color)] text-white transform scale-105 relative'
                : 'bg-md-surface border-2 hover:border-[var(--primary-color)] hover:shadow-2xl',
              menuData.selectedPlan?.id === plan.id
                ? 'ring-4 ring-[var(--accent3-color)] ring-opacity-50'
                : 'border-[var(--light-border-color)]'
            ]"
          >
            <!-- Popular Badge -->
            <div
              v-if="plan.popular"
              class="absolute -top-4 left-1/2 transform -translate-x-1/2 bg-[var(--accent4-color)] text-[var(--dark-text-color)] px-4 py-1 rounded-full text-sm font-bold"
            >
              MOST POPULAR
            </div>

            <!-- Trial Badge -->
            <div class="flex justify-center mb-4">
              <div :class="[
                'inline-flex items-center px-4 py-2 rounded-full text-sm font-bold text-white',
                plan.popular
                  ? 'bg-md-surface bg-opacity-20 text-white'
                  : 'bg-[var(--accent3-color)] bg-opacity-10 text-[var(--accent3-color)]'
              ]">
                <i class="fas fa-gift mr-2 text-white"></i>
                {{ plan.trialDays }}-Day Free Trial
              </div>
            </div>

            <div class="text-center mb-6">
              <h3 :class="[
                'text-2xl font-bold font-[var(--font-family-heading)] mb-2',
                plan.popular ? '' : 'text-[var(--dark-text-color)]'
              ]">
                {{ plan.displayName }}
              </h3>
              <p :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
                {{ plan.description }}
              </p>
            </div>

            <div class="text-center mb-6">
              <div class="flex items-baseline justify-center gap-2">
                <span class="text-sm" :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
                  R
                </span>
                <span :class="['text-5xl font-bold', plan.popular ? '' : 'text-[var(--dark-text-color)]']">
                  {{ plan.price }}
                </span>
                <span :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">/month</span>
              </div>
              <p class="text-sm mt-2" :class="plan.popular ? 'text-white/80' : 'text-[var(--gray-text-color)]'">
                After trial ends
              </p>
            </div>

            <ul class="space-y-3 mb-8">
              <li
                v-for="(feature, fIndex) in plan.features"
                :key="fIndex"
                class="flex items-start gap-3"
              >
                <i :class="[
                  'fas fa-check-circle mt-1',
                  plan.popular ? 'text-white' : 'text-[var(--accent3-color)]'
                ]"></i>
                <span :class="plan.popular ? 'text-white' : 'text-[var(--dark-text-color)]'">
                  {{ feature }}
                </span>
              </li>
            </ul>

            <button
              :class="[
                'w-full px-6 py-3 rounded-lg transition-all font-semibold',
                plan.popular
                  ? 'bg-md-surface text-[var(--primary-color)] hover:bg-opacity-90 shadow-lg'
                  : 'btn-gradient text-white hover:bg-[var(--primary-button-hover-bg-color)]'
              , 'shadow-md-2 hover:shadow-md-4']"
            >
              <i class="fas fa-check mr-2"></i>
              {{ menuData.selectedPlan?.id === plan.id ? 'Selected' : 'Select Plan' }}
            </button>
          </div>
        </div>

        <!-- Next Button -->
        <div class="flex justify-center">
          <button
            @click="nextStep"
            :disabled="!canProceedToNextStep"
            :class="[
              'px-8 py-4 rounded-lg font-bold text-lg transition-all',
              canProceedToNextStep
                ? 'btn-gradient text-white hover:bg-[var(--primary-button-hover-bg-color)] shadow-lg'
                : 'bg-[var(--light-border-color)] text-[var(--gray-text-color)] cursor-not-allowed'
            , 'shadow-md-2 hover:shadow-md-4']"
          >
            Continue to Menu Setup
            <i class="fas fa-arrow-right ml-2"></i>
          </button>
        </div>
      </div>

      <!-- Step 2: Menu Profile Setup -->
      <MenuProfileSetup
        v-else-if="currentStep === 2"
        @next="nextStep"
        @previous="previousStep"
      />

      <!-- Step 3: Categories Setup -->
      <MenuCategoriesSetup
        v-else-if="currentStep === 3"
        @next="nextStep"
        @previous="previousStep"
      />

      <!-- Step 4: Menu Items Setup -->
      <MenuItemsSetup
        v-else-if="currentStep === 4"
        :is-submitting="isSubmitting"
        @previous="previousStep"
        @complete="handleComplete"
      />
    </div>
  </NuxtLayout>
</template>

<script setup>
definePageMeta({
  middleware: 'auth'
})

const router = useRouter()
const {
  menuData,
  currentStep,
  menuPlans,
  selectPlan,
  nextStep,
  previousStep,
  canProceedToNextStep,
  resetMenuCreation
} = useMenuCreation()

const menusApi = useMenusApi()
const uploadsApi = useUploadsApi()
const isSubmitting = ref(false)

const getStepLabel = (step) => {
  const labels = {
    1: 'Choose Plan',
    2: 'Menu Info',
    3: 'Categories',
    4: 'Menu Items'
  }
  return labels[step]
}

const handlePlanSelect = (plan) => {
  selectPlan(plan)
}

const handleComplete = async () => {
  if (isSubmitting.value) return

  isSubmitting.value = true

  try {
    const { $trackEvent } = useNuxtApp()

    // Upload business logo if provided
    let logoUrl = menuData.value.menuProfile.businessLogoUrl
    if (menuData.value.menuProfile.businessLogo && menuData.value.menuProfile.businessLogo instanceof File) {
      try {
        const logoResponse = await uploadsApi.uploadLogo(menuData.value.menuProfile.businessLogo)
        logoUrl = logoResponse.data.url
      } catch (error) {
        console.error('Failed to upload logo:', error)
      }
    }

    // Upload menu item images
    const itemsWithUploadedImages = await Promise.all(
      menuData.value.menuItems.map(async (item) => {
        if (item.image && item.image instanceof File) {
          try {
            const imageResponse = await uploadsApi.uploadMenuImage(item.image)
            return {
              ...item,
              imageUrl: imageResponse.data.url,
              image: undefined // Remove the File object
            }
          } catch (error) {
            console.error(`Failed to upload image for ${item.name}:`, error)
            return { ...item, image: undefined }
          }
        }
        return { ...item, image: undefined }
      })
    )

    // Prepare menu data for API
    const menuPayload = {
      name: menuData.value.menuProfile.name,
      description: menuData.value.menuProfile.description,
      businessName: menuData.value.menuProfile.businessName,
      businessLogo: logoUrl,
      cuisine: menuData.value.menuProfile.cuisine,
      phoneNumber: menuData.value.menuProfile.phoneNumber,
      email: menuData.value.menuProfile.email,
      address: menuData.value.menuProfile.address,
      city: menuData.value.menuProfile.city,
      country: menuData.value.menuProfile.country,
      workingHours: menuData.value.menuProfile.workingHours,
      // SEO fields
      enableSEO: menuData.value.menuProfile.enableSEO,
      slug: menuData.value.menuProfile.slug,
      metaTitle: menuData.value.menuProfile.metaTitle,
      metaDescription: menuData.value.menuProfile.metaDescription,
      keywords: menuData.value.menuProfile.keywords,
      categories: menuData.value.categories.map(cat => ({
        name: cat.name,
        description: cat.description,
        icon: cat.icon,
        order: cat.order
      })),
      items: itemsWithUploadedImages.map(item => ({
        categoryId: item.categoryId,
        name: item.name,
        description: item.description,
        price: item.price,
        imageUrl: item.imageUrl,
        allergens: item.allergens,
        dietary: item.dietary,
        available: item.available,
        featured: item.featured
      })),
      subscriptionPlan: {
        planId: menuData.value.selectedPlan.id,
        planName: menuData.value.selectedPlan.name,
        price: menuData.value.selectedPlan.price
      },
      trial: menuData.value.trial
    }

    // Save menu to backend
    const response = await menusApi.createMenu(menuPayload)

    // Track successful menu creation
    if ($trackEvent) {
      $trackEvent('Menu Created', {
        menuName: menuData.value.menuProfile.name,
        categoriesCount: menuData.value.categories.length,
        itemsCount: menuData.value.menuItems.length,
        plan: menuData.value.selectedPlan.name
      })
    }

    // Reset menu creation state
    resetMenuCreation()

    // Navigate to dashboard with success message
    router.push('/dashboard?menuCreated=true')
  } catch (error) {
    console.error('Failed to create menu:', error)
    const toast = useToast()
    toast.error('Failed to create menu. Please try again.', 'Error')
  } finally {
    isSubmitting.value = false
  }
}

useHead({
  title: 'Create Menu Profile',
})
</script>




