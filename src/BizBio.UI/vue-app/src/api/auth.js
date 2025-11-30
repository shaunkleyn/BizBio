import apiClient from './client'

export default {
  register(data) {
    return apiClient.post('/auth/register', data)
  },

  login(credentials) {
    return apiClient.post('/auth/login', credentials)
  },

  verifyEmail(token) {
    return apiClient.get(`/auth/verify-email?token=${token}`)
  },

  resendVerification(email) {
    return apiClient.post('/auth/resend-verification', { email })
  },

  forgotPassword(email) {
    return apiClient.post('/auth/forgot-password', { email })
  },

  resetPassword(token, newPassword) {
    return apiClient.post('/auth/reset-password', { token, newPassword })
  }
}
