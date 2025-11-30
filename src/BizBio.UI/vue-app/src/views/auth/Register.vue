<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const error = ref('')
const loading = ref(false)
const success = ref(false)

const handleRegister = async () => {
  error.value = ''

  // Validation
  if (form.value.password !== form.value.confirmPassword) {
    error.value = 'Passwords do not match'
    return
  }

  if (form.value.password.length < 8) {
    error.value = 'Password must be at least 8 characters long'
    return
  }

  loading.value = true

  const result = await authStore.register({
    firstName: form.value.firstName,
    lastName: form.value.lastName,
    email: form.value.email,
    password: form.value.password
  })

  loading.value = false

  if (result.success) {
    success.value = true
    // Optionally redirect to login or show success message
  } else {
    error.value = result.error
  }
}
</script>

<template>
  <div class="min-h-full flex bg-gray-50">
    <!-- Left Side - Registration Form -->
    <div class="flex-1 flex flex-col justify-center py-12 px-4 sm:px-6 lg:flex-none lg:px-20 xl:px-24">
      <div class="mx-auto w-full max-w-sm lg:w-96">
        <div>
          <a href="/" class="text-3xl font-heading font-bold text-gray-900">
            <i class="fas fa-qrcode text-primary mr-2"></i>BizBio
          </a>
          <h2 class="mt-6 text-3xl font-heading font-bold text-brand-dark-text">Create your account</h2>
          <p class="mt-2 text-sm text-brand-gray-text">
            Already have an account?
            <router-link to="/login" class="font-semibold text-primary hover:text-primary-600">
              Sign in
            </router-link>
          </p>
        </div>

        <div class="mt-8">
          <!-- Success Alert -->
          <div v-if="success" class="mb-6 p-4 bg-green-50 border-l-4 border-green-500 rounded-lg">
            <div class="flex items-start">
              <i class="fas fa-check-circle text-green-500 mt-0.5 mr-3 text-lg"></i>
              <div>
                <p class="text-green-800 font-semibold text-sm">Registration successful!</p>
                <p class="text-green-700 text-xs mt-1">
                  We've sent a verification email to <strong>{{ form.email }}</strong>.
                  Please check your inbox and click the verification link to activate your account.
                </p>
                <p class="text-green-600 text-xs mt-2">
                  <i class="fas fa-info-circle mr-1"></i>
                  <strong>Don't see it?</strong> Check your spam or junk folder.
                </p>
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

          <!-- Registration Form -->
          <form v-if="!success" @submit.prevent="handleRegister" class="space-y-6">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label for="first-name" class="block text-sm font-medium text-gray-900">
                  First name
                </label>
                <input
                  type="text"
                  id="first-name"
                  v-model="form.firstName"
                  required
                  autocomplete="given-name"
                  class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
                  placeholder="John"
                />
              </div>
              <div>
                <label for="last-name" class="block text-sm font-medium text-gray-900">
                  Last name
                </label>
                <input
                  type="text"
                  id="last-name"
                  v-model="form.lastName"
                  required
                  autocomplete="family-name"
                  class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
                  placeholder="Doe"
                />
              </div>
            </div>

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
                autocomplete="new-password"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
                placeholder="Minimum 8 characters"
              />
              <p class="mt-1 text-xs text-gray-500">Must be at least 8 characters</p>
            </div>

            <div>
              <label for="confirm-password" class="block text-sm font-medium text-gray-900">
                Confirm password
              </label>
              <input
                type="password"
                id="confirm-password"
                v-model="form.confirmPassword"
                required
                autocomplete="new-password"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-primary focus:border-primary transition-all"
                placeholder="Re-enter your password"
              />
            </div>

            <div class="flex items-start">
              <input
                id="terms"
                name="terms"
                type="checkbox"
                required
                class="h-4 w-4 mt-0.5 text-primary focus:ring-primary border-gray-300 rounded"
              />
              <label for="terms" class="ml-2 block text-sm text-gray-900">
                I agree to the
                <a href="/terms" class="text-primary hover:text-primary-600">Terms of Service</a>
                and
                <a href="/privacy" class="text-primary hover:text-primary-600">Privacy Policy</a>
              </label>
            </div>

            <div>
              <button
                type="submit"
                :disabled="loading"
                class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-semibold text-white bg-primary hover:bg-primary-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:opacity-50 disabled:cursor-not-allowed transition-all"
              >
                <i v-if="loading" class="fas fa-spinner fa-spin mr-2"></i>
                <i v-else class="fas fa-user-plus mr-2"></i>
                {{ loading ? 'Creating account...' : 'Create account' }}
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
          <h2 class="text-4xl font-heading font-bold mb-4">Join 500+ South African Businesses</h2>
          <p class="text-xl mb-8 text-white/90">Start your free 14-day trial today</p>
          <div class="space-y-4 text-left">
            <div class="flex items-center">
              <i class="fas fa-check-circle mr-3 text-2xl"></i>
              <span class="text-lg">No credit card required</span>
            </div>
            <div class="flex items-center">
              <i class="fas fa-check-circle mr-3 text-2xl"></i>
              <span class="text-lg">Cancel anytime</span>
            </div>
            <div class="flex items-center">
              <i class="fas fa-check-circle mr-3 text-2xl"></i>
              <span class="text-lg">Full access to all features</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
