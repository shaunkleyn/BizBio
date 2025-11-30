<template>
  <div>
    <section
      class="min-h-screen bg-gradient-to-br from-[var(--light-background-color)] to-white flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 relative overflow-hidden">
      <!-- Decorative Background Blobs -->
      <div class="absolute top-20 right-10 w-72 h-72 bg-[var(--primary-color)] rounded-full opacity-10 blur-3xl"></div>
      <div class="absolute bottom-20 left-10 w-96 h-96 bg-[var(--accent3-color)] rounded-full opacity-10 blur-3xl">
      </div>

      <div class="max-w-md w-full relative z-10">
        <!-- Header -->
        <div class="text-center mb-8">
          <div class="flex justify-center mb-6">
            <div class="bg-[var(--primary-color)] bg-opacity-10 rounded-full p-4">
              <i class="fal fa text-white text-5xl"></i>
            </div>
          </div>
          <h1
            class="text-3xl sm:text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
            Welcome Back
          </h1>
          <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
            Sign in to access your digital business profiles
          </p>
        </div>

        <!-- Login Form -->
        <div class="bg-white rounded-2xl shadow-2xl p-8">
          <!-- Error Message -->
          <div v-if="error"
            class="mb-6 bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-lg p-4">
            <div class="flex items-center gap-3">
              <i class="fas fa-exclamation-circle text-[var(--accent-color)] text-xl"></i>
              <p class="text-[var(--dark-text-color)]">{{ error }}</p>
            </div>
          </div>

          <!-- Unverified Email Message -->
          <div v-if="showResendVerification"
            class="mb-6 bg-[var(--accent4-color)] bg-opacity-10 border-2 border-[var(--accent4-color)] rounded-lg p-4">
            <div class="flex items-center gap-3 mb-3">
              <i class="fas fa-envelope text-[var(--accent4-color)] text-xl"></i>
              <p class="text-[var(--dark-text-color)] text-sm">Please verify your email address to continue.</p>
            </div>
            <button @click="handleResendVerification" :disabled="resendLoading"
              class="text-[var(--primary-color)] hover:underline text-sm font-semibold">
              <i v-if="resendLoading" class="fas fa-spinner fa-spin mr-1"></i>
              {{ resendLoading ? 'Sending...' : 'Resend verification email' }}
            </button>
          </div>

          <!-- Success Message -->
          <div v-if="resendSuccess"
            class="mb-6 bg-[var(--accent3-color)] bg-opacity-10 border-2 border-[var(--accent3-color)] rounded-lg p-4">
            <div class="flex items-center gap-3">
              <i class="fas fa-check-circle text-[var(--accent3-color)] text-xl"></i>
              <p class="text-[var(--dark-text-color)] text-sm">Verification email sent successfully!</p>
            </div>
          </div>

          <form @submit.prevent="handleLogin" class="space-y-6">
            <!-- Email Field -->
            <div>
              <label for="email"
                class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                Email Address
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <i class="fas fa-envelope text-[var(--gray-text-color)]"></i>
                </div>
                <input type="email" id="email" v-model="formData.email" required
                  class="block w-full pl-12 pr-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                  placeholder="your@email.com" />
              </div>
            </div>

            <!-- Password Field -->
            <div>
              <div class="flex items-center justify-between mb-2">
                <label for="password"
                  class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)]">
                  Password
                </label>
                <NuxtLink to="/forgot-password"
                  class="text-sm text-[var(--primary-color)] hover:underline font-[var(--font-family-body)]">
                  Forgot password?
                </NuxtLink>
              </div>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <i class="fas fa-lock text-[var(--gray-text-color)]"></i>
                </div>
                <input type="password" id="password" v-model="formData.password" required
                  class="block w-full pl-12 pr-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                  placeholder="••••••••" />
              </div>
            </div>

            <!-- Remember Me Checkbox -->
            <div class="flex items-center">
              <input type="checkbox" id="remember" v-model="rememberMe"
                class="h-5 w-5 text-[var(--primary-color)] border-[var(--light-border-color)] rounded focus:ring-[var(--primary-color)]" />
              <label for="remember" class="ml-3 text-sm text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                Remember me for 30 days
              </label>
            </div>

            <!-- Submit Button -->
            <button type="submit" :disabled="loading"
              class="w-full bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)] shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2">
              <i v-if="loading" class="fas fa-spinner fa-spin"></i>
              <i v-else class="fas fa-sign-in-alt"></i>
              <span>{{ loading ? 'Signing In...' : 'Sign In' }}</span>
            </button>
          </form>

          <!-- Sign Up Link -->
          <div class="mt-8 text-center">
            <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
              Don't have an account?
              <NuxtLink to="/register" class="text-[var(--primary-color)] hover:underline font-semibold">
                Create Account
              </NuxtLink>
            </p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()

const formData = ref({
  email: '',
  password: ''
})

const rememberMe = ref(false)
const loading = ref(false)
const error = ref(null)
const showResendVerification = ref(false)
const resendLoading = ref(false)
const resendSuccess = ref(false)

const handleLogin = async () => {
  error.value = null
  showResendVerification.value = false
  resendSuccess.value = false
  loading.value = true

  const result = await authStore.login(formData.value)

  if (result.success) {
    // Redirect to dashboard or the page they were trying to access
    const redirect = route.query.redirect || '/dashboard'
    router.push(String(redirect))
  } else {
    error.value = result.error

    // Check if error is about email verification
    if (result.error?.toLowerCase().includes('verify') || result.error?.toLowerCase().includes('email')) {
      showResendVerification.value = true
    }
  }

  loading.value = false
}

const handleResendVerification = async () => {
  if (!formData.value.email) {
    error.value = 'Please enter your email address'
    return
  }

  resendLoading.value = true
  resendSuccess.value = false

  const result = await authStore.resendVerification(formData.value.email)

  if (result.success) {
    resendSuccess.value = true
    showResendVerification.value = false
    error.value = null
  } else {
    error.value = result.error
  }

  resendLoading.value = false
}

useHead({
  title: 'Login',
})
</script>
