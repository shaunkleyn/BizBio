import apiClient from './client'

export default {
  // Get subscription tiers
  getTiers() {
    return apiClient.get('/subscriptions/tiers')
  },

  // Get current subscription
  getCurrentSubscription() {
    return apiClient.get('/subscriptions/current')
  },

  // Subscribe to a tier
  subscribe(tierId, billingCycle) {
    return apiClient.post('/subscriptions/subscribe', { tierId, billingCycle })
  },

  // Update subscription
  updateSubscription(subscriptionId, tierId) {
    return apiClient.put(`/subscriptions/${subscriptionId}`, { tierId })
  },

  // Cancel subscription
  cancel(subscriptionId) {
    return apiClient.post(`/subscriptions/${subscriptionId}/cancel`)
  },

  // Get subscription history
  getHistory() {
    return apiClient.get('/subscriptions/history')
  },

  // Process PayFast payment
  processPayment(data) {
    return apiClient.post('/payments/payfast', data)
  },

  // Verify PayFast payment
  verifyPayment(paymentId) {
    return apiClient.get(`/payments/verify/${paymentId}`)
  }
}
