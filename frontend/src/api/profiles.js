import apiClient from './client'

export default {
  // Get profile by slug
  getBySlug(slug) {
    return apiClient.get(`/profiles/${slug}`)
  },

  // Get current user's profiles
  getMyProfiles() {
    return apiClient.get('/profiles/my')
  },

  // Create new profile
  create(data) {
    return apiClient.post('/profiles', data)
  },

  // Update profile
  update(id, data) {
    return apiClient.put(`/profiles/${id}`, data)
  },

  // Delete profile
  delete(id) {
    return apiClient.delete(`/profiles/${id}`)
  },

  // Track profile view
  trackView(slug) {
    return apiClient.post(`/profiles/${slug}/track-view`)
  },

  // Get profile analytics
  getAnalytics(id) {
    return apiClient.get(`/profiles/${id}/analytics`)
  }
}
