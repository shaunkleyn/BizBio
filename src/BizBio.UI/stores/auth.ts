import { defineStore } from 'pinia'

interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  [key: string]: any
}

interface AuthState {
  user: User | null
  token: string | null
  loading: boolean
  error: string | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    user: null,
    token: null,
    loading: false,
    error: null
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    currentUser: (state) => state.user
  },

  actions: {
    // Initialize auth state from localStorage
    initAuth() {
      if (import.meta.client) {
        const storedUser = localStorage.getItem('user')
        const storedToken = localStorage.getItem('token')

        if (storedUser && storedToken) {
          this.user = JSON.parse(storedUser)
          this.token = storedToken
        }
      }
    },

    async register(userData: any) {
      this.loading = true
      this.error = null

      try {
        const authApi = useAuthApi()
        const response = await authApi.register(userData)
        return { success: true, data: response }
      } catch (error: any) {
        this.error = error.response?.data?.message || error.message || 'Registration failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async login(credentials: any) {
      this.loading = true
      this.error = null

      try {
        const authApi = useAuthApi()
        const response = await authApi.login(credentials)

        // Check if response has success property
        if (response.success === false) {
          this.error = response.message || response.error || 'Login failed'
          return { success: false, error: this.error }
        }

        // Handle different response structures
        // Could be { success: true, data: { user, token } } or { success: true, user, token }
        let user, token
        if (response.data) {
          // Structure: { success: true, data: { user, token } }
          user = response.data.user
          token = response.data.token
        } else {
          // Structure: { success: true, user, token }
          user = response.user
          token = response.token
        }

        if (!user || !token) {
          this.error = 'Invalid response from server'
          return { success: false, error: this.error }
        }

        this.user = user
        this.token = token

        if (import.meta.client) {
          localStorage.setItem('user', JSON.stringify(user))
          localStorage.setItem('token', token)
        }

        return { success: true, data: { user, token } }
      } catch (error: any) {
        this.error = error.response?.data?.message || error.message || 'Login failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async verifyEmail(token: string) {
      this.loading = true
      this.error = null

      try {
        const authApi = useAuthApi()
        const response = await authApi.verifyEmail(token)
        return { success: true, data: response }
      } catch (error: any) {
        this.error = error.response?.data?.message || error.message || 'Email verification failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async resendVerification(email: string) {
      this.loading = true
      this.error = null

      try {
        const authApi = useAuthApi()
        const response = await authApi.resendVerification(email)
        return { success: true, data: response }
      } catch (error: any) {
        this.error = error.response?.data?.message || error.message || 'Failed to resend verification email'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async forgotPassword(email: string) {
      this.loading = true
      this.error = null

      try {
        const authApi = useAuthApi()
        const response = await authApi.forgotPassword(email)
        return { success: true, data: response }
      } catch (error: any) {
        this.error = error.response?.data?.message || error.message || 'Password reset request failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async resetPassword(token: string, newPassword: string) {
      this.loading = true
      this.error = null

      try {
        const authApi = useAuthApi()
        const response = await authApi.resetPassword(token, newPassword)
        return { success: true, data: response }
      } catch (error: any) {
        this.error = error.response?.data?.message || error.message || 'Password reset failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    logout() {
      this.user = null
      this.token = null

      if (import.meta.client) {
        localStorage.removeItem('user')
        localStorage.removeItem('token')
      }
    }
  }
})
