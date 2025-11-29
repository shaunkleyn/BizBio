<template>
  <div>
    <section class="min-h-screen bg-gradient-to-br from-[var(--light-background-color)] to-white flex items-center justify-center py-12 px-4">
      <div class="max-w-md w-full">
        <div class="text-center mb-8">
          <div class="flex justify-center mb-6">
            <div class="bg-[var(--accent2-color)] bg-opacity-10 rounded-full p-4">
              <i class="fas fa-key text-[var(--accent2-color)] text-5xl"></i>
            </div>
          </div>
          <h1 class="text-3xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
            Forgot Password?
          </h1>
          <p class="text-[var(--gray-text-color)]">
            Enter your email to receive a password reset link
          </p>
        </div>

        <div class="bg-white rounded-2xl shadow-2xl p-8">
          <div v-if="success" class="text-center py-6">
            <i class="fas fa-check-circle text-6xl text-[var(--accent3-color)] mb-4"></i>
            <p class="text-[var(--dark-text-color)]">Check your email for a password reset link!</p>
          </div>

          <form v-else @submit.prevent="handleSubmit" class="space-y-6">
            <div v-if="error" class="bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-lg p-4">
              <p class="text-[var(--dark-text-color)]">{{ error }}</p>
            </div>

            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">Email Address</label>
              <input
                type="email"
                v-model="email"
                required
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)]"
              />
            </div>

            <button
              type="submit"
              :disabled="loading"
              class="w-full bg-[var(--primary-color)] text-white px-6 py-4 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold disabled:opacity-50"
            >
              {{ loading ? 'Sending...' : 'Send Reset Link' }}
            </button>

            <div class="text-center">
              <NuxtLink to="/login" class="text-[var(--primary-color)] hover:underline">
                Back to Login
              </NuxtLink>
            </div>
          </form>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const authStore = useAuthStore()
const email = ref('')
const loading = ref(false)
const error = ref(null)
const success = ref(false)

const handleSubmit = async () => {
  loading.value = true
  error.value = null

  const result = await authStore.forgotPassword(email.value)

  if (result.success) {
    success.value = true
  } else {
    error.value = result.error
  }

  loading.value = false
}

useHead({ title: 'Forgot Password' })
</script>
