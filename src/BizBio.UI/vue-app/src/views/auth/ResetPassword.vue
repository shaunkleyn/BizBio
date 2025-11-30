<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const token = ref('')
const password = ref('')
const confirmPassword = ref('')
const error = ref('')
const loading = ref(false)
const success = ref(false)

onMounted(() => {
  token.value = route.query.token || ''

  if (!token.value) {
    error.value = 'No reset token provided. Please request a new password reset link.'
  }
})

const handleSubmit = async () => {
  error.value = ''

  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match'
    return
  }

  if (password.value.length < 8) {
    error.value = 'Password must be at least 8 characters long'
    return
  }

  loading.value = true

  const result = await authStore.resetPassword(token.value, password.value)
  loading.value = false

  if (result.success) {
    success.value = true
    // Redirect to login after 3 seconds
    setTimeout(() => {
      router.push('/login')
    }, 3000)
  } else {
    error.value = result.error
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div class="text-center">
        <a href="/" class="text-3xl font-heading font-bold text-gray-900 inline-block">
          <i class="fas fa-qrcode text-primary mr-2"></i>BizBio
        </a>
        <div class="mt-6">
          <i class="fas fa-lock text-primary text-5xl mb-4"></i>
          <h2 class="text-3xl font-heading font-bold text-brand-dark-text">
            Reset Password
          </h2>
          <p class="mt-2 text-sm text-brand-gray-text">
            Create a new password for your account
          </p>
        </div>
      </div>

      <div class="bg-white py-8 px-6 shadow rounded-lg">
        <!-- Success Alert -->
        <div v-if="success" class="mb-6 p-4 bg-green-50 border-l-4 border-green-500 rounded-lg">
          <div class="flex items-start">
            <i class="fas fa-check-circle text-green-500 mt-0.5 mr-3 text-lg"></i>
            <div>
              <p class="text-green-800 font-semibold text-sm">Password reset successful!</p>
              <p class="text-green-700 text-xs mt-1">
                Your password has been updated. You can now login with your new password.
              </p>
              <p class="text-green-600 text-xs mt-2">Redirecting to login page...</p>
              <router-link
                to="/login"
                class="mt-3 inline-block text-sm font-semibold text-green-700 hover:text-green-800"
              >
                Go to Login →
              </router-link>
            </div>
          </div>
        </div>

        <!-- Error Alert -->
        <div v-if="error && !success" class="mb-6 p-4 bg-red-50 border-l-4 border-red-500 rounded-lg">
          <div class="flex items-start">
            <i class="fas fa-exclamation-circle text-red-500 mt-0.5 mr-3"></i>
            <p class="text-red-800 text-sm font-medium">{{ error }}</p>
          </div>
        </div>

        <!-- Form -->
        <form v-if="!success && token" @submit.prevent="handleSubmit" class="space-y-6">
          <div>
            <label for="password" class="block text-sm font-medium text-gray-900">
              New password
            </label>
            <input
              type="password"
              id="password"
              v-model="password"
              required
              autocomplete="new-password"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
              placeholder="Minimum 8 characters"
            />
            <p class="mt-1 text-xs text-gray-500">Must be at least 8 characters</p>
          </div>

          <div>
            <label for="confirm-password" class="block text-sm font-medium text-gray-900">
              Confirm new password
            </label>
            <input
              type="password"
              id="confirm-password"
              v-model="confirmPassword"
              required
              autocomplete="new-password"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
              placeholder="Re-enter your password"
            />
          </div>

          <div>
            <button
              type="submit"
              :disabled="loading"
              class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-semibold text-white bg-primary hover:bg-primary-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:opacity-50 disabled:cursor-not-allowed transition-all"
            >
              <i v-if="loading" class="fas fa-spinner fa-spin mr-2"></i>
              <i v-else class="fas fa-key mr-2"></i>
              {{ loading ? 'Resetting...' : 'Reset Password' }}
            </button>
          </div>
        </form>

        <div v-if="!success" class="mt-6">
          <div class="relative">
            <div class="absolute inset-0 flex items-center">
              <div class="w-full border-t border-gray-300"></div>
            </div>
            <div class="relative flex justify-center text-sm">
              <span class="px-2 bg-white text-gray-500">or</span>
            </div>
          </div>

          <div class="mt-6 text-center">
            <p class="text-sm text-gray-600">
              Remember your password?
              <router-link to="/login" class="font-semibold text-primary hover:text-primary-600">
                Sign in
              </router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
