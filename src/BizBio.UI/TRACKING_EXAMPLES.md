<!-- Example: Enhanced Login Page with Tracking -->
<!-- This shows how to integrate tracking into the login page -->

<script setup>
const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()
const tracking = useTracking()

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

// Track page view on mount
onMounted(() => {
  tracking.trackPageView('/login', 'Login Page')
})

const handleLogin = async () => {
  error.value = null
  showResendVerification.value = false
  resendSuccess.value = false
  loading.value = true

  const startTime = Date.now()

  try {
    const result = await authStore.login(formData.value)

    if (result.success) {
      // Track successful login
      tracking.trackAuth('login', 'email', true)
      
      const duration = Date.now() - startTime
      tracking.trackPerformance('login_duration', duration, 'ms')

      // Redirect to dashboard or the page they were trying to access
      const redirect = route.query.redirect || '/dashboard'
      router.push(String(redirect))
    } else {
      // Track failed login
      tracking.trackAuth('login', 'email', false)
      tracking.trackEvent('login_failed', {
        error_type: result.error?.includes('verify') ? 'unverified_email' : 'invalid_credentials',
        remember_me: rememberMe.value
      })

      error.value = result.error

      // Check if error is about email verification
      if (result.error?.toLowerCase().includes('verify') || result.error?.toLowerCase().includes('email')) {
        showResendVerification.value = true
      }
    }
  } catch (err) {
    // Track exception
    tracking.trackException(err, {
      action: 'login',
      email: formData.value.email
    })
    error.value = 'An unexpected error occurred. Please try again.'
  } finally {
    loading.value = false
  }
}

const handleResendVerification = async () => {
  if (!formData.value.email) {
    error.value = 'Please enter your email address'
    return
  }

  resendLoading.value = true
  resendSuccess.value = false

  try {
    const result = await authStore.resendVerification(formData.value.email)

    if (result.success) {
      resendSuccess.value = true
      showResendVerification.value = false
      error.value = null
      
      // Track successful resend
      tracking.trackEvent('verification_email_resent', {
        email: formData.value.email,
        success: true
      })
    } else {
      error.value = result.error
      
      // Track failed resend
      tracking.trackEvent('verification_email_resent', {
        email: formData.value.email,
        success: false,
        error: result.error
      })
    }
  } catch (err) {
    tracking.trackException(err, {
      action: 'resend_verification',
      email: formData.value.email
    })
    error.value = 'Failed to resend verification email'
  } finally {
    resendLoading.value = false
  }
}

// Track when user clicks forgot password
const trackForgotPasswordClick = () => {
  tracking.trackUserAction('click', 'forgot_password_link', {
    page: 'login'
  })
}

// Track remember me toggle
watch(rememberMe, (newValue) => {
  tracking.trackUserAction('toggle', 'remember_me', {
    enabled: newValue
  })
})

useHead({
  title: 'Login',
})
</script>

<!-- Template remains the same, just add tracking to forgot password link -->
<!-- 
<NuxtLink to="/forgot-password" @click="trackForgotPasswordClick"
  class="text-sm text-[var(--primary-color)] hover:underline font-[var(--font-family-body)]">
  Forgot password?
</NuxtLink>
-->
