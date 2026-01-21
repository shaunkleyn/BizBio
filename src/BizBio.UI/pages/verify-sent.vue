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
              <i class="fas fa-envelope-open-text text-[var(--accent3-color)] text-5xl"></i>
            </div>
          </div>
          <h1 class="text-3xl sm:text-4xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-2">
            Check Your Email
          </h1>
          <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
            We've sent a verification email to <strong>{{ email }}</strong>
          </p>
        </div>

        <!-- Main Content -->
        <div class="bg-md-surface rounded-2xl shadow-2xl p-8 sm:p-10">
          <!-- Resend Success Banner -->
          <div v-if="resendSuccess" class="mb-6 bg-[var(--accent3-color)] bg-opacity-10 border-2 border-[var(--accent3-color)] rounded-lg p-4">
            <div class="flex items-center gap-3">
              <i class="fas fa-paper-plane text-[var(--accent3-color)] text-2xl"></i>
              <div>
                <p class="font-semibold text-[var(--dark-text-color)]">New Verification Email Sent!</p>
                <p class="text-sm text-[var(--gray-text-color)] mt-1">A fresh verification code has been sent to your email address.</p>
              </div>
            </div>
          </div>

          <div class="text-center mb-8">
            <div class="inline-block bg-[var(--accent3-color)] bg-opacity-10 rounded-full p-6 mb-4">
              <i class="fas fa-check-circle text-[var(--accent3-color)] text-6xl"></i>
            </div>
            <h2 class="text-2xl font-bold text-[var(--dark-text-color)] font-[var(--font-family-heading)] mb-4">
              Verification Email Sent!
            </h2>
            <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] mb-6">
              Please verify your email address before signing in. You have two options to verify:
            </p>
          </div>

          <!-- Verification Options -->
          <div class="space-y-6 mb-8">
            <!-- Option 1: Enter Code -->
            <div class="border-2 border-[var(--primary-color)] rounded-lg p-6">
              <div class="flex items-start gap-4">
                <div class="flex-shrink-0">
                  <div class="bg-[var(--primary-color)] text-white rounded-full w-8 h-8 flex items-center justify-center font-bold">
                    1
                  </div>
                </div>
                <div class="flex-1">
                  <h3 class="font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                    Enter the 6-digit verification code
                  </h3>
                  <p class="text-sm text-[var(--gray-text-color)] mb-4">
                    Check your email for a 6-digit code and enter it below to verify your account.
                  </p>
                  <button
                    @click="showCodeInput = true"
                    v-if="!showCodeInput"
                    class="bg-[var(--primary-color)] text-[var(--primary-button-text-color)] px-6 py-2 rounded-[var(--button-rounded-radius)] hover:bg-[var(--primary-button-hover-bg-color)] transition-all duration-200 font-semibold font-[var(--font-family-body)]"
                  >
                    Enter Code
                  </button>

                  <!-- Code Input Form -->
                  <div v-if="showCodeInput" class="mt-4">
                    <form @submit.prevent="verifyCode" class="space-y-4">
                      <div>
                        <label for="code" class="block text-sm font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                          Verification Code
                        </label>
                        <input
                          type="text"
                          id="code"
                          v-model="verificationCode"
                          maxlength="6"
                          pattern="[0-9]{6}"
                          required
                          class="block w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)] transition-colors font-[var(--font-family-body)] text-center text-2xl tracking-widest"
                          placeholder="000000"
                          @input="formatCode"
                        />
                      </div>

                      <!-- Error Message -->
                      <div v-if="error" class="bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-lg p-4">
                        <div class="flex items-center gap-3">
                          <i class="fas fa-exclamation-circle text-[var(--accent-color)]"></i>
                          <p class="text-[var(--dark-text-color)] text-sm">{{ error }}</p>
                        </div>
                      </div>

                      <div class="flex gap-3">
                        <button
                          type="submit"
                          :disabled="loading || verificationCode.length !== 6"
                          class="flex-1 bg-[var(--accent3-color)] text-white px-6 py-3 rounded-[var(--button-rounded-radius)] hover:opacity-90 transition-all duration-200 font-semibold font-[var(--font-family-body)] disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                        >
                          <i v-if="loading" class="fas fa-spinner fa-spin"></i>
                          <span>{{ loading ? 'Verifying...' : 'Verify Code' }}</span>
                        </button>
                        <button
                          type="button"
                          @click="showCodeInput = false; verificationCode = ''; error = null"
                          :disabled="loading"
                          class="px-6 py-3 border-2 border-[var(--light-border-color)] rounded-[var(--button-rounded-radius)] hover:bg-gray-50 transition-all duration-200 font-semibold font-[var(--font-family-body)] disabled:opacity-50"
                        >
                          Cancel
                        </button>
                      </div>
                    </form>
                  </div>
                </div>
              </div>
            </div>

            <!-- Option 2: Click Link -->
            <div class="border-2 border-[var(--light-border-color)] rounded-lg p-6">
              <div class="flex items-start gap-4">
                <div class="flex-shrink-0">
                  <div class="bg-[var(--gray-text-color)] text-white rounded-full w-8 h-8 flex items-center justify-center font-bold">
                    2
                  </div>
                </div>
                <div class="flex-1">
                  <h3 class="font-semibold text-[var(--dark-text-color)] font-[var(--font-family-body)] mb-2">
                    Click the verification link in your email
                  </h3>
                  <p class="text-sm text-[var(--gray-text-color)]">
                    Alternatively, you can click the verification button in the email we sent you.
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Additional Info -->
          <div class="bg-[var(--light-background-color)] rounded-lg p-4 mb-6">
            <div class="flex items-start gap-3">
              <i class="fas fa-info-circle text-[var(--primary-color)] mt-1"></i>
              <div class="text-sm text-[var(--gray-text-color)]">
                <p><strong>Didn't receive the email?</strong></p>
                <ul class="list-disc ml-5 mt-2 space-y-1">
                  <li>Check your spam or junk folder</li>
                  <li>Make sure you entered the correct email address</li>
                  <li>Wait a few minutes and check again</li>
                </ul>
              </div>
            </div>
          </div>

          <!-- Resend Email -->
          <div class="text-center">
            <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)] mb-4">
              Still didn't receive it?
            </p>
            <button
              @click="resendVerification"
              :disabled="resending || resendCooldown > 0"
              class="text-[var(--primary-color)] hover:underline font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <i v-if="resending" class="fas fa-spinner fa-spin mr-2"></i>
              {{ resendCooldown > 0 ? `Resend in ${resendCooldown}s` : 'Resend verification email' }}
            </button>

            <!-- Resend Error Message -->
            <div v-if="resendError" class="mt-4 bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-lg p-4">
              <div class="flex items-center gap-3">
                <i class="fas fa-exclamation-circle text-[var(--accent-color)]"></i>
                <p class="text-[var(--dark-text-color)] text-sm">{{ resendError }}</p>
              </div>
            </div>
          </div>

          <!-- Back to Login -->
          <div class="mt-8 text-center pt-6 border-t border-[var(--light-border-color)]">
            <p class="text-[var(--gray-text-color)] font-[var(--font-family-body)]">
              Already verified your email?
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

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const email = ref(route.query.email || '')
const verificationCode = ref('')
const showCodeInput = ref(false)
const loading = ref(false)
const error = ref(null)
const resending = ref(false)
const resendSuccess = ref(false)
const resendError = ref(null)
const resendCooldown = ref(0)

// Redirect to register if no email provided
onMounted(() => {
  if (!email.value) {
    router.push('/register')
  }

  // Check if this is from a resend action
  if (route.query.resend === 'true') {
    resendSuccess.value = true
    // Clear the resend query parameter
    router.replace({ query: { email: email.value } })
  }
})

const formatCode = (event) => {
  // Only allow digits
  verificationCode.value = verificationCode.value.replace(/\D/g, '')
}

const verifyCode = async () => {
  error.value = null
  loading.value = true

  try {
    const authApi = useAuthApi()
    const response = await authApi.verifyEmailWithCode(email.value, verificationCode.value)

    // Success - redirect to login
    router.push('/login?verified=true')
  } catch (err) {
    error.value = err.response?.data?.message || 'Verification failed. Please try again.'
  } finally {
    loading.value = false
  }
}

const resendVerification = async () => {
  if (!email.value) return

  resending.value = true
  resendSuccess.value = false
  resendError.value = null

  try {
    await authStore.resendVerification(email.value)
    resendSuccess.value = true
    resendError.value = null

    // Start cooldown timer
    resendCooldown.value = 60
    const interval = setInterval(() => {
      resendCooldown.value--
      if (resendCooldown.value <= 0) {
        clearInterval(interval)
      }
    }, 1000)
  } catch (err) {
    resendError.value = err.response?.data?.message || 'Failed to resend verification email. Please try again.'
    resendSuccess.value = false
  } finally {
    resending.value = false
  }
}

useHead({
  title: 'Verify Your Email',
})
</script>
