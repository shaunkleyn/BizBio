export const useSubscriptionApi = () => {
  const api = useApi()

  return {
    // Tier operations
    getAllTiers: () => api.get('/api/v1/subscription-tiers'),
    getTiersByProduct: (productType: number) => api.get(`/api/v1/subscription-tiers/product/${productType}`),

    // Subscription operations (correct routes)
    getMySubscriptions: () => api.get('/api/v1/subscriptions'),
    getSubscriptionByProduct: (productType: number) => api.get(`/api/v1/subscriptions/${productType}`),
    subscribe: (data: any) => api.post('/api/v1/subscriptions', data),
    upgradeSubscription: (productType: number, data: any) => api.put(`/api/v1/subscriptions/${productType}/upgrade`, data),
    cancelSubscription: (productType: number) => api.post(`/api/v1/subscriptions/${productType}/cancel`),
    getInvoicePreview: () => api.get('/api/v1/subscriptions/invoice-preview')
  }
}
