<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(true)
const success = ref(false)
const error = ref('')

onMounted(async () => {
  const token = route.query.token

  if (!token) {
    error.value = 'No verification token provided'
    loading.value = false
    return
  }

  const result = await authStore.verifyEmail(token)
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
})
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full">
      <div class="text-center mb-8">
        <a href="/" class="text-3xl font-heading font-bold text-gray-900 inline-block">
          <i class="fas fa-qrcode text-primary mr-2"></i>BizBio
        </a>
      </div>

      <div class="bg-white py-8 px-6 shadow rounded-lg text-center">
        <!-- Loading State -->
        <div v-if="loading" class="py-8">
          <i class="fas fa-spinner fa-spin text-primary text-6xl mb-4"></i>
          <h3 class="text-2xl font-heading font-bold text-brand-dark-text mb-2">
            Verifying your email...
          </h3>
          <p class="text-brand-gray-text">
            Please wait while we verify your email address.
          </p>
        </div>

        <!-- Success State -->
        <div v-else-if="success" class="py-8">
          <i class="fas fa-check-circle text-green-500 text-6xl mb-4"></i>
          <h3 class="text-2xl font-heading font-bold text-green-600 mb-2">
            Email Verified!
          </h3>
          <p class="text-brand-gray-text mt-3 mb-4">
            Your email has been successfully verified.
            You can now login to your account.
          </p>
          <p class="text-sm text-gray-500 mb-6">Redirecting to login page...</p>
          <router-link
            to="/login"
            class="inline-flex items-center justify-center px-6 py-3 border border-transparent rounded-lg shadow-sm text-sm font-semibold text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 transition-all"
          >
            <i class="fas fa-sign-in-alt mr-2"></i>
            Go to Login
          </router-link>
        </div>

        <!-- Error State -->
        <div v-else class="py-8">
          <i class="fas fa-times-circle text-red-500 text-6xl mb-4"></i>
          <h3 class="text-2xl font-heading font-bold text-red-600 mb-2">
            Verification Failed
          </h3>
          <p class="text-brand-gray-text mt-3 mb-6">{{ error }}</p>
          <div class="flex flex-col sm:flex-row gap-3 justify-center">
            <router-link
              to="/login"
              class="inline-flex items-center justify-center px-6 py-3 border border-transparent rounded-lg shadow-sm text-sm font-semibold text-white bg-primary hover:bg-primary-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary transition-all"
            >
              <i class="fas fa-sign-in-alt mr-2"></i>
              Go to Login
            </router-link>
            <router-link
              to="/register"
              class="inline-flex items-center justify-center px-6 py-3 border border-gray-300 rounded-lg shadow-sm text-sm font-semibold text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary transition-all"
            >
              <i class="fas fa-user-plus mr-2"></i>
              Register Again
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
