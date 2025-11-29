import axios, { type AxiosInstance } from 'axios'

let apiClient: AxiosInstance | null = null

export const useApi = () => {
  const config = useRuntimeConfig()
  const router = useRouter()

  if (apiClient) {
    return apiClient
  }

  apiClient = axios.create({
    baseURL: config.public.apiUrl as string,
    headers: {
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*'
    },
    withCredentials: false,
  })

  // Add request interceptor to include JWT token
  apiClient.interceptors.request.use(
    (config) => {
      if (import.meta.client) {
        const token = localStorage.getItem('token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
      }
      return config
    },
    (error) => {
      return Promise.reject(error)
    }
  )

  // Add response interceptor for error handling
  apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
      // Only redirect to login if:
      // 1. We got a 401 error
      // 2. It's NOT from the login endpoint
      // 3. It's NOT from the register endpoint
      const isAuthEndpoint = error.config?.url?.includes('/auth/login') ||
                            error.config?.url?.includes('/auth/register')

      if (error.response?.status === 401 && !isAuthEndpoint && import.meta.client) {
        // Token expired or invalid, redirect to login
        localStorage.removeItem('token')
        localStorage.removeItem('user')
        router.push('/login')
      }
      return Promise.reject(error)
    }
  )

  return apiClient
}

// Auth API
export const useAuthApi = () => {
  const api = useApi()

  return {
    register: (data: any) => api.post('/auth/register', data),
    login: (credentials: any) => api.post('/auth/login', credentials),
    verifyEmail: (token: string) => api.get(`/auth/verify-email?token=${token}`),
    resendVerification: (email: string) => api.post('/auth/resend-verification', { email }),
    forgotPassword: (email: string) => api.post('/auth/forgot-password', { email }),
    resetPassword: (token: string, newPassword: string) =>
      api.post('/auth/reset-password', { token, newPassword })
  }
}

// Profiles API
export const useProfilesApi = () => {
  const api = useApi()

  return {
    getBySlug: (slug: string) => api.get(`/profiles/${slug}`),
    getMyProfiles: () => api.get('/profiles'),
    create: (data: any) => api.post('/profiles', data),
    update: (id: string, data: any) => api.put(`/profiles/${id}`, data),
    delete: (id: string) => api.delete(`/profiles/${id}`),
    trackView: (slug: string) => api.post(`/profiles/${slug}/view`),
    getAnalytics: (id: string) => api.get(`/profiles/${id}/analytics`)
  }
}

// Menus API
export const useMenusApi = () => {
  const api = useApi()

  return {
    getByProfileSlug: (profileSlug: string) => api.get(`/menus/${profileSlug}`),
    getItems: (menuId: string) => api.get(`/menus/${menuId}/items`),
    createItem: (menuId: string, data: any) => api.post(`/menus/${menuId}/items`, data),
    updateItem: (menuId: string, itemId: string, data: any) =>
      api.put(`/menus/${menuId}/items/${itemId}`, data),
    deleteItem: (menuId: string, itemId: string) =>
      api.delete(`/menus/${menuId}/items/${itemId}`),
    getCategories: (menuId: string) => api.get(`/menus/${menuId}/categories`),
    reorderItems: (menuId: string, data: any) => api.put(`/menus/${menuId}/reorder`, data)
  }
}

// Tables API
export const useTablesApi = () => {
  const api = useApi()

  return {
    getMyTables: () => api.get('/tables'),
    getById: (id: string) => api.get(`/tables/${id}`),
    getByNFCCode: (code: string) => api.get(`/tables/nfc/${code}`),
    create: (data: any) => api.post('/tables', data),
    update: (id: string, data: any) => api.put(`/tables/${id}`, data),
    delete: (id: string) => api.delete(`/tables/${id}`),
    generateNFCCode: (id: string) => api.post(`/tables/${id}/generate-nfc`)
  }
}

// Subscriptions API
export const useSubscriptionsApi = () => {
  const api = useApi()

  return {
    getTiers: () => api.get('/subscriptions/tiers'),
    getCurrentSubscription: () => api.get('/subscriptions/current'),
    subscribe: (tierId: string, paymentData: any) =>
      api.post('/subscriptions/subscribe', { tierId, ...paymentData }),
    updateSubscription: (tierId: string) => api.put('/subscriptions/update', { tierId }),
    cancel: () => api.post('/subscriptions/cancel'),
    getHistory: () => api.get('/subscriptions/history'),
    processPayment: (data: any) => api.post('/payments/process', data),
    verifyPayment: (paymentId: string) => api.get(`/payments/verify/${paymentId}`)
  }
}

// Uploads API
export const useUploadsApi = () => {
  const api = useApi()

  return {
    uploadFile: (file: File) => {
      const formData = new FormData()
      formData.append('file', file)
      return api.post('/uploads', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      })
    },
    uploadLogo: (file: File) => {
      const formData = new FormData()
      formData.append('logo', file)
      return api.post('/uploads/logo', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      })
    },
    uploadMenuImage: (file: File) => {
      const formData = new FormData()
      formData.append('image', file)
      return api.post('/uploads/menu-image', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      })
    },
    deleteFile: (fileId: string) => api.delete(`/uploads/${fileId}`)
  }
}
