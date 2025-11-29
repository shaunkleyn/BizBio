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
        return { success: true, data: response.data }
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Registration failed'
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
        const { user, token } = response.data

        this.user = user
        this.token = token

        if (import.meta.client) {
          localStorage.setItem('user', JSON.stringify(user))
          localStorage.setItem('token', token)
        }

        return { success: true, data: response.data }
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Login failed'
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
        return { success: true, data: response.data }
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Email verification failed'
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
        return { success: true, data: response.data }
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to resend verification email'
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
        return { success: true, data: response.data }
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Password reset request failed'
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
        return { success: true, data: response.data }
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Password reset failed'
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
