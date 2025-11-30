import apiClient from './client'

export default {
  // Upload file (image, logo, etc.)
  uploadFile(file, type = 'image') {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('type', type)

    return apiClient.post('/uploads', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  },

  // Upload profile logo
  uploadLogo(file) {
    return this.uploadFile(file, 'logo')
  },

  // Upload menu item image
  uploadMenuImage(file) {
    return this.uploadFile(file, 'menu-item')
  },

  // Delete uploaded file
  deleteFile(fileId) {
    return apiClient.delete(`/uploads/${fileId}`)
  }
}
