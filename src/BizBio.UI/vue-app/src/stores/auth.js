import { defineStore } from 'pinia'
import authApi from '../api/auth'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('token') || null,
    loading: false,
    error: null
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    currentUser: (state) => state.user
  },

  actions: {
    async register(userData) {
      this.loading = true
      this.error = null

      try {
        const response = await authApi.register(userData)
        return { success: true, data: response.data }
      } catch (error) {
        this.error = error.response?.data?.message || 'Registration failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async login(credentials) {
      this.loading = true
      this.error = null

      try {
        const response = await authApi.login(credentials)
        const { user, token } = response.data

        this.user = user
        this.token = token

        localStorage.setItem('user', JSON.stringify(user))
        localStorage.setItem('token', token)

        return { success: true, data: response.data }
      } catch (error) {
        this.error = error.response?.data?.message || 'Login failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async verifyEmail(token) {
      this.loading = true
      this.error = null

      try {
        const response = await authApi.verifyEmail(token)
        return { success: true, data: response.data }
      } catch (error) {
        this.error = error.response?.data?.message || 'Email verification failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async resendVerification(email) {
      this.loading = true
      this.error = null

      try {
        const response = await authApi.resendVerification(email)
        return { success: true, data: response.data }
      } catch (error) {
        this.error = error.response?.data?.message || 'Failed to resend verification email'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async forgotPassword(email) {
      this.loading = true
      this.error = null

      try {
        const response = await authApi.forgotPassword(email)
        return { success: true, data: response.data }
      } catch (error) {
        this.error = error.response?.data?.message || 'Password reset request failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async resetPassword(token, newPassword) {
      this.loading = true
      this.error = null

      try {
        const response = await authApi.resetPassword(token, newPassword)
        return { success: true, data: response.data }
      } catch (error) {
        this.error = error.response?.data?.message || 'Password reset failed'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    logout() {
      this.user = null
      this.token = null
      localStorage.removeItem('user')
      localStorage.removeItem('token')
    }
  }
})
