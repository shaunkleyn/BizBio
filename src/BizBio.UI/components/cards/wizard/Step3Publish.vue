<template>
  <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
    <div class="text-center mb-10">
      <div class="w-16 h-16 bg-gradient-secondary rounded-full flex items-center justify-center mx-auto mb-4 shadow-glow-pink">
        <i class="fas fa-check text-white text-2xl"></i>
      </div>
      <h2 class="text-2xl sm:text-3xl font-bold text-md-on-surface mb-2">Your card is live!</h2>
      <p class="text-md-on-surface-variant">Share it with the world</p>
    </div>

    <div class="grid md:grid-cols-2 gap-8">
      <!-- Left: Profile URL and sharing -->
      <div class="space-y-6">
        <!-- Profile URL -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-6">
          <h3 class="font-bold text-md-on-surface mb-4 flex items-center gap-2">
            <i class="fas fa-link text-md-secondary"></i>
            Your Profile URL
          </h3>
          <div class="flex items-center gap-2 p-3 bg-md-surface-container rounded-xl mb-3">
            <i class="fas fa-globe text-md-on-surface-variant text-sm flex-shrink-0"></i>
            <span class="flex-1 text-sm text-md-on-surface font-medium truncate">
              {{ profileUrl }}
            </span>
          </div>
          <div class="flex gap-2">
            <button
              class="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl border border-md-outline text-md-on-surface hover:bg-md-surface-container transition-all text-sm font-medium"
              @click="copyUrl"
            >
              <i :class="copied ? 'fas fa-check text-green-500' : 'fas fa-copy'" class="text-sm"></i>
              {{ copied ? 'Copied!' : 'Copy Link' }}
            </button>
            <button
              class="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl btn-gradient text-white shadow-md-2 hover:shadow-md-4 transition-all text-sm font-medium"
              @click="shareUrl"
            >
              <i class="fas fa-share-alt text-sm"></i>
              Share
            </button>
          </div>
        </div>

        <!-- Analytics teaser -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-6">
          <h3 class="font-bold text-md-on-surface mb-3 flex items-center gap-2">
            <i class="fas fa-chart-bar text-md-secondary"></i>
            Analytics
          </h3>

          <div v-if="state.selectedPlan === 'free'" class="relative">
            <!-- Blurred placeholder -->
            <div class="space-y-2 mb-4 blur-sm pointer-events-none select-none">
              <div class="flex items-center justify-between p-3 bg-md-surface-container rounded-lg">
                <span class="text-sm text-md-on-surface">Profile views this week</span>
                <span class="font-bold text-md-on-surface">247</span>
              </div>
              <div class="flex items-center justify-between p-3 bg-md-surface-container rounded-lg">
                <span class="text-sm text-md-on-surface">Unique visitors</span>
                <span class="font-bold text-md-on-surface">189</span>
              </div>
            </div>
            <div class="absolute inset-0 flex items-center justify-center">
              <div class="bg-md-surface rounded-xl shadow-md-3 p-4 text-center max-w-[80%]">
                <i class="fas fa-lock text-md-secondary text-xl mb-2"></i>
                <p class="text-sm font-medium text-md-on-surface">Upgrade to Solo to see analytics</p>
                <p class="text-xs text-md-on-surface-variant mt-1">From R59/mo</p>
              </div>
            </div>
          </div>

          <div v-else class="space-y-2">
            <div class="flex items-center justify-between p-3 bg-md-surface-container rounded-lg">
              <span class="text-sm text-md-on-surface">Card just created</span>
              <span class="text-xs text-md-on-surface-variant">Stats available soon</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Right: QR code -->
      <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-6 flex flex-col items-center">
        <h3 class="font-bold text-md-on-surface mb-4 flex items-center gap-2 self-start">
          <i class="fas fa-qrcode text-md-secondary"></i>
          QR Code
        </h3>

        <div v-if="state.qrCodeSvg" class="mb-4">
          <div
            class="p-4 bg-white rounded-2xl shadow-md-2"
            v-html="state.qrCodeSvg"
          ></div>
        </div>
        <div v-else class="w-48 h-48 bg-md-surface-container rounded-2xl flex items-center justify-center mb-4">
          <i class="fas fa-qrcode text-md-on-surface-variant text-5xl"></i>
        </div>

        <p class="text-xs text-md-on-surface-variant text-center mb-4">
          Scan this QR code to open your digital business card
        </p>

        <div class="flex gap-2 w-full">
          <button
            class="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl border border-md-outline text-md-on-surface hover:bg-md-surface-container transition-all text-sm font-medium"
            @click="downloadQr('svg')"
          >
            <i class="fas fa-download text-sm"></i>
            SVG
          </button>
          <button
            class="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl btn-gradient text-white shadow-md-2 hover:shadow-md-4 transition-all text-sm font-medium"
            @click="downloadQr('png')"
          >
            <i class="fas fa-download text-sm"></i>
            PNG
          </button>
        </div>
      </div>
    </div>

    <!-- Finish button -->
    <div class="text-center mt-10">
      <NuxtLink
        to="/dashboard/cards"
        class="inline-flex items-center gap-3 btn-gradient px-10 py-4 rounded-2xl text-white font-bold text-lg shadow-md-4 hover:shadow-md-5 transition-all"
      >
        <i class="fas fa-th-large"></i>
        Go to My Cards
      </NuxtLink>
      <p class="text-sm text-md-on-surface-variant mt-3">
        You can edit your card anytime from the dashboard
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { WizardState } from '~/composables/useBusinessCardCreation'

const props = defineProps<{
  state: WizardState
}>()

const copied = ref(false)

const profileUrl = computed(() =>
  props.state.profileUrl || `https://bizbio.co.za/${props.state.slug}`
)

const copyUrl = async () => {
  try {
    await navigator.clipboard.writeText(profileUrl.value)
    copied.value = true
    setTimeout(() => { copied.value = false }, 2000)
  } catch {
    // Fallback
  }
}

const shareUrl = async () => {
  if (navigator.share) {
    try {
      await navigator.share({
        title: 'My Digital Business Card',
        url: profileUrl.value,
      })
    } catch { /* cancelled */ }
  } else {
    await copyUrl()
  }
}

const downloadQr = (format: 'svg' | 'png') => {
  if (!props.state.qrCodeSvg) return

  if (format === 'svg') {
    const blob = new Blob([props.state.qrCodeSvg], { type: 'image/svg+xml' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `${props.state.slug || 'qr-code'}.svg`
    a.click()
    URL.revokeObjectURL(url)
  } else {
    // Convert SVG to PNG via canvas
    const img = new Image()
    const svgBlob = new Blob([props.state.qrCodeSvg], { type: 'image/svg+xml' })
    const url = URL.createObjectURL(svgBlob)
    img.onload = () => {
      const canvas = document.createElement('canvas')
      canvas.width = 400
      canvas.height = 400
      const ctx = canvas.getContext('2d')
      if (ctx) {
        ctx.fillStyle = 'white'
        ctx.fillRect(0, 0, 400, 400)
        ctx.drawImage(img, 0, 0, 400, 400)
        const pngUrl = canvas.toDataURL('image/png')
        const a = document.createElement('a')
        a.href = pngUrl
        a.download = `${props.state.slug || 'qr-code'}.png`
        a.click()
      }
      URL.revokeObjectURL(url)
    }
    img.src = url
  }
}
</script>
