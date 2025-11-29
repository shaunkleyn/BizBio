<script setup>
import { ref } from 'vue'
import { useAuthStore } from '../../stores/auth'

const authStore = useAuthStore()

const email = ref('')
const error = ref('')
const loading = ref(false)
const success = ref(false)

const handleSubmit = async () => {
  error.value = ''
  loading.value = true

  const result = await authStore.forgotPassword(email.value)
  loading.value = false

  if (result.success) {
    success.value = true
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
          <i class="fas fa-key text-primary text-5xl mb-4"></i>
          <h2 class="text-3xl font-heading font-bold text-brand-dark-text">
            Forgot Password?
          </h2>
          <p class="mt-2 text-sm text-brand-gray-text">
            Enter your email address and we'll send you a link to reset your password
          </p>
        </div>
      </div>

      <div class="bg-white py-8 px-6 shadow rounded-lg">
        <!-- Success Alert -->
        <div v-if="success" class="mb-6 p-4 bg-green-50 border-l-4 border-green-500 rounded-lg">
          <div class="flex items-start">
            <i class="fas fa-check-circle text-green-500 mt-0.5 mr-3 text-lg"></i>
            <div>
              <p class="text-green-800 font-semibold text-sm">Email sent!</p>
              <p class="text-green-700 text-xs mt-1">
                If an account exists with {{ email }}, you will receive a password reset link shortly.
                Please check your email and spam folder.
              </p>
              <router-link
                to="/login"
                class="mt-3 inline-block text-sm font-semibold text-green-700 hover:text-green-800"
              >
                Back to Login →
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
        <form v-if="!success" @submit.prevent="handleSubmit" class="space-y-6">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-900">
              Email address
            </label>
            <input
              type="email"
              id="email"
              v-model="email"
              required
              autocomplete="email"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
              placeholder="your@email.com"
            />
          </div>

          <div>
            <button
              type="submit"
              :disabled="loading"
              class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-semibold text-white bg-primary hover:bg-primary-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:opacity-50 disabled:cursor-not-allowed transition-all"
            >
              <i v-if="loading" class="fas fa-spinner fa-spin mr-2"></i>
              <i v-else class="fas fa-paper-plane mr-2"></i>
              {{ loading ? 'Sending...' : 'Send Reset Link' }}
            </button>
          </div>
        </form>

        <div class="mt-6">
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
