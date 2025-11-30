<script setup>
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const form = ref({
  email: '',
  password: ''
})

const error = ref('')
const loading = ref(false)
const resendingVerification = ref(false)
const verificationResent = ref(false)

const showResendButton = computed(() => {
  return error.value && (
    error.value.toLowerCase().includes('email not verified') ||
    error.value.toLowerCase().includes('verify your email')
  )
})

const handleLogin = async () => {
  error.value = ''
  verificationResent.value = false
  loading.value = true

  const result = await authStore.login(form.value)

  loading.value = false

  if (result.success) {
    const redirect = route.query.redirect || '/dashboard'
    router.push(redirect)
  } else {
    error.value = result.error
  }
}

const handleResendVerification = async () => {
  if (!form.value.email) {
    error.value = 'Please enter your email address'
    return
  }

  resendingVerification.value = true
  verificationResent.value = false

  const result = await authStore.resendVerification(form.value.email)

  resendingVerification.value = false

  if (result.success) {
    verificationResent.value = true
    error.value = ''
  } else {
    error.value = result.error
  }
}
</script>

<template>
  <div class="min-h-full flex bg-gray-50">
    <!-- Left Side - Login Form -->
    <div class="flex-1 flex flex-col justify-center py-12 px-4 sm:px-6 lg:flex-none lg:px-20 xl:px-24">
      <div class="mx-auto w-full max-w-sm lg:w-96">
        <div>
          <a href="/" class="text-3xl font-heading font-bold text-gray-900">
            <i class="fas fa-qrcode text-primary mr-2"></i>BizBio
          </a>
          <h2 class="mt-6 text-3xl font-heading font-bold text-brand-dark-text">Welcome back</h2>
          <p class="mt-2 text-sm text-brand-gray-text">
            Don't have an account?
            <router-link to="/register" class="font-semibold text-primary hover:text-primary-600">
              Create account
            </router-link>
          </p>
        </div>

        <div class="mt-8">
          <!-- Error Alert -->
          <div v-if="error" class="mb-6 p-4 bg-red-50 border-l-4 border-red-500 rounded-lg">
            <div class="flex items-start">
              <i class="fas fa-exclamation-circle text-red-500 mt-0.5 mr-3"></i>
              <div class="flex-1">
                <p class="text-red-800 text-sm font-medium">{{ error }}</p>
                <button
                  v-if="showResendButton"
                  @click="handleResendVerification"
                  :disabled="resendingVerification"
                  class="mt-2 text-sm text-red-600 hover:text-red-700 font-semibold flex items-center gap-2"
                >
                  <i :class="resendingVerification ? 'fas fa-spinner fa-spin' : 'fas fa-envelope'"></i>
                  {{ resendingVerification ? 'Sending...' : 'Resend Verification Email' }}
                </button>
              </div>
            </div>
          </div>

          <!-- Success Alert -->
          <div v-if="verificationResent" class="mb-6 p-4 bg-green-50 border-l-4 border-green-500 rounded-lg">
            <div class="flex items-start">
              <i class="fas fa-check-circle text-green-500 mt-0.5 mr-3"></i>
              <div>
                <p class="text-green-800 font-semibold text-sm">Verification email sent!</p>
                <p class="text-green-700 text-xs mt-0.5">Check your inbox and spam folder.</p>
              </div>
            </div>
          </div>

          <!-- Login Form -->
          <form @submit.prevent="handleLogin" class="space-y-6">
            <div>
              <label for="email" class="block text-sm font-medium text-gray-900">
                Email address
              </label>
              <input
                type="email"
                id="email"
                v-model="form.email"
                required
                autocomplete="email"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
                placeholder="your@email.com"
              />
            </div>

            <div>
              <label for="password" class="block text-sm font-medium text-gray-900">
                Password
              </label>
              <input
                type="password"
                id="password"
                v-model="form.password"
                required
                autocomplete="current-password"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
                placeholder="Enter your password"
              />
            </div>

            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <input
                  id="remember-me"
                  name="remember-me"
                  type="checkbox"
                  class="h-4 w-4 text-primary focus:ring-primary border-gray-300 rounded"
                />
                <label for="remember-me" class="ml-2 block text-sm text-gray-900">
                  Remember me
                </label>
              </div>

              <div class="text-sm">
                <router-link to="/forgot-password" class="font-semibold text-primary hover:text-primary-600">
                  Forgot password?
                </router-link>
              </div>
            </div>

            <div>
              <button
                type="submit"
                :disabled="loading"
                class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-semibold text-white bg-primary hover:bg-primary-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:opacity-50 disabled:cursor-not-allowed transition-all"
              >
                <i v-if="loading" class="fas fa-spinner fa-spin mr-2"></i>
                <i v-else class="fas fa-sign-in-alt mr-2"></i>
                {{ loading ? 'Signing in...' : 'Sign in' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Right Side - Branding -->
    <div class="hidden lg:block relative w-0 flex-1">
      <div class="absolute inset-0 bg-gradient-to-br from-primary to-accent-purple flex items-center justify-center p-12">
        <div class="text-white text-center max-w-md">
          <h2 class="text-4xl font-heading font-bold mb-4">Your Digital Business Solution</h2>
          <p class="text-xl mb-8 text-white/90">Manage your digital presence with ease</p>
          <div class="space-y-4 text-left">
            <div class="flex items-center">
              <i class="fas fa-check-circle mr-3 text-2xl"></i>
              <span class="text-lg">Digital Business Cards</span>
            </div>
            <div class="flex items-center">
              <i class="fas fa-check-circle mr-3 text-2xl"></i>
              <span class="text-lg">Smart Restaurant Menus</span>
            </div>
            <div class="flex items-center">
              <i class="fas fa-check-circle mr-3 text-2xl"></i>
              <span class="text-lg">Product Catalogs & More</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
