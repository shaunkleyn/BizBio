<template>
  <div>
    <section class="min-h-screen bg-gradient-to-br from-[var(--light-background-color)] to-white flex items-center justify-center py-12 px-4">
      <div class="max-w-md w-full">
        <div class="text-center mb-8">
          <h1 class="text-3xl font-bold text-[var(--dark-text-color)] mb-2">Reset Password</h1>
          <p class="text-[var(--gray-text-color)]">Enter your new password</p>
        </div>

        <div class="bg-white rounded-2xl shadow-2xl p-8">
          <form @submit.prevent="handleSubmit" class="space-y-6">
            <div v-if="error" class="bg-[var(--accent-color)] bg-opacity-10 border-2 border-[var(--accent-color)] rounded-lg p-4">
              <p class="text-[var(--dark-text-color)]">{{ error }}</p>
            </div>

            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">New Password</label>
              <input
                type="password"
                v-model="password"
                required
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)]"
              />
            </div>

            <div>
              <label class="block text-sm font-semibold text-[var(--dark-text-color)] mb-2">Confirm Password</label>
              <input
                type="password"
                v-model="confirmPassword"
                required
                class="w-full px-4 py-3 border-2 border-[var(--light-border-color)] rounded-lg focus:ring-2 focus:ring-[var(--primary-color)] focus:border-[var(--primary-color)]"
              />
            </div>

            <button
              type="submit"
              :disabled="loading"
              class="w-full bg-[var(--primary-color)] text-white px-6 py-4 rounded-lg hover:bg-[var(--primary-button-hover-bg-color)] transition-colors font-semibold disabled:opacity-50"
            >
              {{ loading ? 'Resetting...' : 'Reset Password' }}
            </button>
          </form>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { useAuthStore } from '~/stores/auth'

const authStore = useAuthStore()
const route = useRoute()
const router = useRouter()

const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const error = ref(null)

const handleSubmit = async () => {
  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match'
    return
  }

  loading.value = true
  error.value = null

  const token = String(route.query.token || '')
  const result = await authStore.resetPassword(token, password.value)

  if (result.success) {
    router.push('/login')
  } else {
    error.value = result.error
  }

  loading.value = false
}

useHead({ title: 'Reset Password' })
</script>
