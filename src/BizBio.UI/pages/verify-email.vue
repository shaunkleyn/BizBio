<template>
  <div>
    <section class="min-h-screen bg-gradient-to-br from-[var(--light-background-color)] to-white flex items-center justify-center py-12 px-4">
      <div class="max-w-md w-full text-center">
        <div v-if="verifying" class="bg-md-surface rounded-2xl shadow-2xl p-12">
          <i class="fas fa-spinner fa-spin text-6xl text-md-primary mb-4"></i>
          <p class="text-xl text-[var(--dark-text-color)]">Verifying your email...</p>
        </div>

        <div v-else-if="success" class="bg-md-surface rounded-2xl shadow-2xl p-12">
          <i class="fas fa-check-circle text-6xl text-[var(--accent3-color)] mb-4"></i>
          <h1 class="text-2xl font-bold text-[var(--dark-text-color)] mb-4">Email Verified!</h1>
          <p class="text-[var(--gray-text-color)] mb-6">Your email has been successfully verified. You can now sign in to your account.</p>
          <p class="text-sm text-[var(--gray-text-color)] mb-6">Redirecting to login...</p>
        </div>

        <div v-else class="bg-md-surface rounded-2xl shadow-2xl p-12">
          <i class="fas fa-times-circle text-6xl text-[var(--accent-color)] mb-4"></i>
          <h1 class="text-2xl font-bold text-[var(--dark-text-color)] mb-4">Verification Failed</h1>
          <p class="text-[var(--gray-text-color)] mb-6">{{ error }}</p>
          <NuxtLink
            to="/login"
            class="inline-block bg-md-primary text-white px-8 py-3 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold"
          >
            Back to Login
          </NuxtLink>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const authStore = useAuthStore()
const route = useRoute()
const router = useRouter()

const verifying = ref(true)
const success = ref(false)
const error = ref(null)

onMounted(async () => {
  const token = String(route.query.token || '')

  if (!token) {
    error.value = 'No verification token provided'
    verifying.value = false
    return
  }

  const result = await authStore.verifyEmail(token)

  if (result.success) {
    success.value = true
    // Redirect to login page with verified flag after 2 seconds
    setTimeout(() => {
      router.push('/login?verified=true')
    }, 2000)
  } else {
    error.value = result.error || 'Verification failed'
  }

  verifying.value = false
})

useHead({ title: 'Verify Email' })
</script>

