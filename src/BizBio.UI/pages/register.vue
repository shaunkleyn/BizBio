<template>
  <div>
    <section class="min-h-screen bg-gradient-to-br from-[var(--light-background-color)] to-white flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 relative overflow-hidden">
      <!-- Decorative Background Blobs -->
      <div class="absolute top-20 right-10 w-72 h-72 bg-[var(--accent3-color)] rounded-full opacity-10 blur-3xl"></div>
      <div class="absolute bottom-20 left-10 w-96 h-96 bg-[var(--primary-color)] rounded-full opacity-10 blur-3xl"></div>

      <div class="max-w-2xl w-full relative z-10">
        <!-- Header -->
        <div class="text-center mb-8">
          <div class="flex justify-center mb-6">
            <div class="bg-[var(--accent3-color)] bg-opacity-10 rounded-full p-4">
              <i class="fas fa-user-plus text-[var(--accent3-color)] text-5xl"></i>
            </div>
          </div>
          <h1 class="text-3xl sm:text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
            Create Your Account
          </h1>
          <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
            Join thousands of professionals sharing their profiles smarter
          </p>
        </div>

        <!-- Registration Form -->
        <div class="bg-white rounded-2xl shadow-2xl p-8 sm:p-10">
          <!-- Success Message -->
          <div v-if="successMessage" class="mb-6 bg-[var(--accent3-color)] bg-opacity-10 border-2 border-[var(--accent3-color)] rounded-lg p-4">
            <div class="flex items-center gap-3">
              <i class="fas fa-check-circle text-[var(--accent3-color)] text-2xl"></i>
              <div>
                <p class="font-semibold text-[var(--dark-text-color)]">{{ successMessage }}</p>
                <p class="text-sm text-[var(--gray-text-color)] mt-1">Please check your email to verify your account.</p>
              </div>
            </div>
          </div>

          <!-- Error Message -->
          <div v-if="error" class="mb-6 bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-lg p-4">
            <div class="flex items-center gap-3">
              <i class="fas fa-exclamation-circle text-[var(--accent-color)] text-2xl"></i>
              <p class="text-[var(--dark-text-color)]">{{ error }}</p>
            </div>
          </div>

          <form @submit.prevent="handleRegister" class="space-y-6">
            <!-- Name Fields -->
            <div class="grid sm:grid-cols-2 gap-4">
              <div>
                <label for="firstName" class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                  First Name
                </label>
                <input
                  type="text"
                  id="firstName"
                  v-model="formData.firstName"
                  required
                  class="block w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                  placeholder="John"
                />
              </div>
              <div>
                <label for="lastName" class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                  Last Name
                </label>
                <input
                  type="text"
                  id="lastName"
                  v-model="formData.lastName"
                  required
                  class="block w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                  placeholder="Smith"
                />
              </div>
            </div>

            <!-- Email Field -->
            <div>
              <label for="email" class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                Email Address
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <i class="fas fa-envelope text-[var(--gray-text-color)]"></i>
                </div>
                <input
                  type="email"
                  id="email"
                  v-model="formData.email"
                  required
                  class="block w-full pl-12 pr-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                  placeholder="your@email.com"
                />
              </div>
            </div>

            <!-- Password Fields -->
            <div class="grid sm:grid-cols-2 gap-4">
              <div>
                <label for="password" class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                  Password
                </label>
                <div class="relative">
                  <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <i class="fas fa-lock text-[var(--gray-text-color)]"></i>
                  </div>
                  <input
                    type="password"
                    id="password"
                    v-model="formData.password"
                    required
                    class="block w-full pl-12 pr-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                    placeholder="••••••••"
                  />
                </div>
              </div>
              <div>
                <label for="confirmPassword" class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                  Confirm Password
                </label>
                <div class="relative">
                  <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <i class="fas fa-lock text-[var(--gray-text-color)]"></i>
                  </div>
                  <input
                    type="password"
                    id="confirmPassword"
                    v-model="confirmPassword"
                    required
                    class="block w-full pl-12 pr-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                    placeholder="••••••••"
                  />
                </div>
              </div>
            </div>

            <!-- Phone Number -->
            <div>
              <label for="phone" class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                Phone Number (Optional)
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <i class="fas fa-phone text-[var(--gray-text-color)]"></i>
                </div>
                <input
                  type="tel"
                  id="phone"
                  v-model="formData.phone"
                  class="block w-full pl-12 pr-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)]"
                  placeholder="+1 (555) 123-4567"
                />
              </div>
            </div>

            <!-- Terms Checkbox -->
            <div class="flex items-start">
              <input
                type="checkbox"
                id="terms"
                v-model="agreeToTerms"
                required
                class="mt-1 h-5 w-5 text-[var(--primary-color)] border-[var(--light-border-color)] rounded focus:ring-[var(--primary-color)]"
              />
              <label for="terms" class="ml-3 text-sm text-[var(--gray-text-color)] font-[var(--font-family-body)]">
                I agree to the
                <NuxtLink to="/terms" class="text-[var(--primary-color)] hover:underline">Terms of Service</NuxtLink>
                and
                <NuxtLink to="/privacy" class="text-[var(--primary-color)] hover:underline">Privacy Policy</NuxtLink>
              </label>
            </div>

            <!-- Submit Button -->
            <button
              type="submit"
              :disabled="loading"
              class="w-full bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-4 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)] shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
              <i v-if="loading" class="fas fa-spinner fa-spin"></i>
              <i v-else class="fas fa-user-plus"></i>
              <span>{{ loading ? 'Creating Account...' : 'Create Account' }}</span>
            </button>
          </form>

          <!-- Sign In Link -->
          <div class="mt-8 text-center">
            <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
              Already have an account?
              <NuxtLink to="/login" class="text-[var(--primary-color)] hover:underline font-semibold">
                Sign In
              </NuxtLink>
            </p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { useAuthStore } from '~/stores/auth'

const authStore = useAuthStore()
const router = useRouter()

const formData = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  phone: ''
})

const confirmPassword = ref('')
const agreeToTerms = ref(false)
const loading = ref(false)
const error = ref(null)
const successMessage = ref(null)

const handleRegister = async () => {
  error.value = null
  successMessage.value = null

  // Validate passwords match
  if (formData.value.password !== confirmPassword.value) {
    error.value = 'Passwords do not match'
    return
  }

  // Validate terms agreement
  if (!agreeToTerms.value) {
    error.value = 'You must agree to the Terms of Service and Privacy Policy'
    return
  }

  loading.value = true

  const result = await authStore.register(formData.value)

  if (result.success) {
    successMessage.value = 'Registration successful!'
    // Clear form
    formData.value = {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      phone: ''
    }
    confirmPassword.value = ''
    agreeToTerms.value = false

    // Optionally redirect to login after a delay
    setTimeout(() => {
      router.push('/login')
    }, 3000)
  } else {
    error.value = result.error
  }

  loading.value = false
}

useHead({
  title: 'Register',
})
</script>
