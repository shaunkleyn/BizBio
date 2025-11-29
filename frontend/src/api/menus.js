import apiClient from './client'

export default {
  // Get menu by profile slug
  getByProfileSlug(slug) {
    return apiClient.get(`/menus/profile/${slug}`)
  },

  // Get menu items for a profile
  getItems(profileId) {
    return apiClient.get(`/menus/${profileId}/items`)
  },

  // Create menu item
  createItem(data) {
    return apiClient.post('/menus/items', data)
  },

  // Update menu item
  updateItem(id, data) {
    return apiClient.put(`/menus/items/${id}`, data)
  },

  // Delete menu item
  deleteItem(id) {
    return apiClient.delete(`/menus/items/${id}`)
  },

  // Get menu categories
  getCategories(profileId) {
    return apiClient.get(`/menus/${profileId}/categories`)
  },

  // Reorder items
  reorderItems(profileId, itemIds) {
    return apiClient.post(`/menus/${profileId}/reorder`, { itemIds })
  }
}
