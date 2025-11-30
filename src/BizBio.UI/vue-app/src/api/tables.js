import apiClient from './client'

export default {
  // Get all tables for current user
  getMyTables() {
    return apiClient.get('/tables/my')
  },

  // Get table by ID
  getById(id) {
    return apiClient.get(`/tables/${id}`)
  },

  // Get table by NFC code
  getByNFCCode(code) {
    return apiClient.get(`/tables/nfc/${code}`)
  },

  // Create table
  create(data) {
    return apiClient.post('/tables', data)
  },

  // Update table
  update(id, data) {
    return apiClient.put(`/tables/${id}`, data)
  },

  // Delete table
  delete(id) {
    return apiClient.delete(`/tables/${id}`)
  },

  // Generate NFC code
  generateNFCCode(tableId) {
    return apiClient.post(`/tables/${tableId}/generate-nfc`)
  }
}
