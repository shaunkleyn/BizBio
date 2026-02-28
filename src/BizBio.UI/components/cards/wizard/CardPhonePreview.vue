<template>
  <div class="flex flex-col items-center h-full">
    <p class="text-xs text-md-on-surface-variant mb-3 font-medium uppercase tracking-wider">Live Preview</p>

    <!-- Phone frame -->
    <div class="relative mx-auto" style="width: 280px;">
      <!-- Phone outer shell -->
      <div
        class="relative bg-gray-900 rounded-[40px] p-3 shadow-2xl"
        style="box-shadow: 0 0 0 2px #1a1a1a, 0 25px 50px -12px rgba(0,0,0,0.6);"
      >
        <!-- Notch -->
        <div class="absolute top-3 left-1/2 -translate-x-1/2 w-20 h-5 bg-gray-900 rounded-full z-10"></div>

        <!-- Screen bezel -->
        <div class="bg-white rounded-[30px] overflow-hidden relative" style="height: 560px;">
          <!-- Status bar -->
          <div class="bg-gray-900 text-white flex items-center justify-between px-6 pt-2 pb-1 text-[10px] absolute top-0 left-0 right-0 z-10" style="padding-top: 22px;">
            <span>9:41</span>
            <div class="flex items-center gap-1">
              <i class="fas fa-signal text-[8px]"></i>
              <i class="fas fa-wifi text-[8px]"></i>
              <i class="fas fa-battery-full text-[8px]"></i>
            </div>
          </div>

          <!-- Card iframe -->
          <iframe
            ref="iframeEl"
            class="w-full h-full border-none"
            sandbox="allow-scripts allow-same-origin"
            title="Card Preview"
          ></iframe>

          <!-- Loading overlay -->
          <div
            v-if="loading"
            class="absolute inset-0 bg-white flex items-center justify-center"
          >
            <div class="text-center">
              <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-md-secondary mb-2"></div>
              <p class="text-xs text-md-on-surface-variant">Updating preview...</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Home indicator -->
      <div class="mx-auto mt-2 w-24 h-1 bg-gray-700 rounded-full"></div>
    </div>

    <p class="text-xs text-md-on-surface-variant mt-3 text-center">
      Changes appear automatically
    </p>
  </div>
</template>

<script setup lang="ts">
import type { WizardState } from '~/composables/useBusinessCardCreation'

const props = defineProps<{
  state: WizardState
}>()

const iframeEl = ref<HTMLIFrameElement>()
const loading = ref(false)
const { buildSrcdoc } = useTemplatePopulator()

let updateTimer: ReturnType<typeof setTimeout> | null = null

const updatePreview = async () => {
  if (!iframeEl.value) return
  loading.value = true
  try {
    const srcdoc = await buildSrcdoc(props.state)
    iframeEl.value.srcdoc = srcdoc
  } catch (err) {
    console.warn('Preview update failed:', err)
  } finally {
    loading.value = false
  }
}

const debouncedUpdate = () => {
  if (updateTimer) clearTimeout(updateTimer)
  updateTimer = setTimeout(updatePreview, 300)
}

// Watch for state changes (deep watch on key fields)
watch(
  () => ({
    template: props.state.selectedTemplate,
    firstName: props.state.firstName,
    lastName: props.state.lastName,
    headline: props.state.headline,
    company: props.state.company,
    photoUrl: props.state.photoUrl,
    bio: props.state.bio,
    buttons: JSON.stringify(props.state.contactButtons),
    info: JSON.stringify(props.state.contactInfo),
    social: JSON.stringify(props.state.socialLinks),
    sections: JSON.stringify(props.state.sections),
    appearance: JSON.stringify(props.state.appearance),
  }),
  () => debouncedUpdate(),
  { deep: true }
)

onMounted(() => {
  updatePreview()
})

onUnmounted(() => {
  if (updateTimer) clearTimeout(updateTimer)
})
</script>
