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
      'Content-Type': 'application/json'
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

        // Add start time for tracking
        config.metadata = { startTime: Date.now() }
      }
      return config
    },
    (error) => {
      return Promise.reject(error)
    }
  )

  // Add response interceptor for error handling
  apiClient.interceptors.response.use(
    (response) => {
      // Track successful API calls
      if (import.meta.client && response.config.metadata?.startTime) {
        try {
          const duration = Date.now() - response.config.metadata.startTime
          const method = response.config.method?.toUpperCase() || 'GET'
          const url = `${response.config.baseURL}${response.config.url}`

          // Use window.$nuxt to avoid SSR issues
          if (typeof window !== 'undefined' && (window as any).$nuxt?.$trackDependency) {
            (window as any).$nuxt.$trackDependency(
              `${method} ${response.config.url}`,
              method,
              url,
              duration,
              true,
              response.status,
              {
                responseSize: JSON.stringify(response.data).length,
                endpoint: response.config.url
              }
            )
          }
        } catch (err) {
          // Fail silently if tracking fails
          console.debug('Tracking error:', err)
        }
      }
      // Return only the data property instead of the full response
      return response.data
    },
    (error) => {
      // Track failed API calls
      if (import.meta.client && error.config?.metadata?.startTime) {
        try {
          const duration = Date.now() - error.config.metadata.startTime
          const method = error.config.method?.toUpperCase() || 'GET'
          const url = `${error.config.baseURL}${error.config.url}`
          const statusCode = error.response?.status || 0

          // Use window.$nuxt to avoid SSR issues
          if (typeof window !== 'undefined' && (window as any).$nuxt?.$trackDependency) {
            (window as any).$nuxt.$trackDependency(
              `${method} ${error.config.url}`,
              method,
              url,
              duration,
              false,
              statusCode,
              {
                errorMessage: error.message,
                endpoint: error.config.url
              }
            )
          }

          // Track as exception for 5xx errors
          if (statusCode >= 500 && typeof window !== 'undefined' && (window as any).$nuxt?.$trackException) {
            (window as any).$nuxt.$trackException(new Error(`API Error: ${method} ${error.config.url} - ${statusCode}`), {
              endpoint: error.config.url,
              statusCode,
              duration,
              errorMessage: error.response?.data?.message || error.message
            })
          }
        } catch (err) {
          // Fail silently if tracking fails
          console.debug('Tracking error:', err)
        }
      }

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
    getMenuBySlug: (slug: string) => api.get(`/c/${slug}`),
    getItemDetails: (slug: string, itemId: number) => api.get(`/c/${slug}/items/${itemId}`),
    getBundleDetails: (slug: string, bundleId: number) => api.get(`/c/${slug}/bundles/${bundleId}`),
    getItems: (menuId: string) => api.get(`/menus/${menuId}/items`),
    createItem: (menuId: string, data: any) => api.post(`/menus/${menuId}/items`, data),
    updateItem: (menuId: string, itemId: string, data: any) =>
      api.put(`/menus/${menuId}/items/${itemId}`, data),
    deleteItem: (menuId: string, itemId: string) =>
      api.delete(`/menus/${menuId}/items/${itemId}`),
    getCategories: (menuId: string) => api.get(`/menus/${menuId}/categories`),
    reorderItems: (menuId: string, data: any) => api.put(`/menus/${menuId}/reorder`, data),
    // Create complete menu with profile, categories, and items
    createMenu: (data: any) => api.post('/menus', data),
    getMyMenus: () => api.get('/menus/my'),
    getMenuById: (menuId: number) => api.get(`/menus/${menuId}`),
    updateMenu: (menuId: string, data: any) => api.put(`/menus/${menuId}`, data),
    deleteMenu: (menuId: string) => api.delete(`/menus/${menuId}`)
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

// Bundles API
export const useBundlesApi = () => {
  const api = useApi()

  return {
    // Bundle operations
    getBundles: (catalogId: string) => api.get(`/dashboard/catalogs/${catalogId}/bundles`),
    getBundle: (catalogId: string, bundleId: string) =>
      api.get(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}`),
    createBundle: (catalogId: string, data: any) =>
      api.post(`/dashboard/catalogs/${catalogId}/bundles`, data),
    updateBundle: (catalogId: string, bundleId: string, data: any) =>
      api.put(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}`, data),
    deleteBundle: (catalogId: string, bundleId: string) =>
      api.delete(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}`),

    // Step operations
    addStep: (catalogId: string, bundleId: string, data: any) =>
      api.post(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}/steps`, data),

    // Product assignment
    addProductToStep: (catalogId: string, bundleId: string, stepId: string, data: any) =>
      api.post(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}/steps/${stepId}/products`, data),

    // Option groups
    addOptionGroup: (catalogId: string, bundleId: string, stepId: string, data: any) =>
      api.post(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}/steps/${stepId}/option-groups`, data),

    // Options
    addOption: (catalogId: string, bundleId: string, optionGroupId: string, data: any) =>
      api.post(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}/option-groups/${optionGroupId}/options`, data),

    // Add bundle to category
    addToCategory: (catalogId: string, bundleId: string, data: any) =>
      api.post(`/dashboard/catalogs/${catalogId}/bundles/${bundleId}/add-to-category`, data)
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

// Library Items API
export const useLibraryItemsApi = () => {
  const api = useApi()

  return {
    // Library items CRUD
    getItems: (categoryId?: number) => {
      const params = categoryId ? `?categoryId=${categoryId}` : ''
      return api.get(`/library/items${params}`)
    },
    getItem: (id: number) => api.get(`/library/items/${id}`),
    createItem: (data: any) => api.post('/library/items', data),
    updateItem: (id: number, data: any) => api.put(`/library/items/${id}`, data),
    deleteItem: (id: number) => api.delete(`/library/items/${id}`),
    addToCatalog: (id: number, data: any) => api.post(`/library/items/${id}/add-to-catalog`, data)
  }
}

// Library Categories API
export const useLibraryCategoriesApi = () => {
  const api = useApi()

  return {
    // Library categories CRUD
    getCategories: () => api.get('/library/categories'),
    getCategory: (id: number) => api.get(`/library/categories/${id}`),
    createCategory: (data: any) => api.post('/library/categories', data),
    updateCategory: (id: number, data: any) => api.put(`/library/categories/${id}`, data),
    deleteCategory: (id: number) => api.delete(`/library/categories/${id}`),
    addToCatalog: (id: number, data: any) => api.post(`/library/categories/${id}/add-to-catalog`, data)
  }
}

// Catalogs API (Menu Editor)
export const useCatalogsApi = () => {
  const api = useApi()

  return {
    // Get full catalog details for editing - uses /menus/{id} endpoint
    getCatalogDetail: (id: number) => api.get(`/menus/${id}`),

    // Category operations
    createCategory: (catalogId: number, dto: any) =>
      api.post(`/catalogs/${catalogId}/categories`, dto),
    updateCategory: (categoryId: number, dto: any) =>
      api.put(`/categories/${categoryId}`, dto),
    deleteCategory: (categoryId: number) =>
      api.delete(`/categories/${categoryId}`),

    // Reorder operations
    reorderCategories: (catalogId: number, items: any) =>
      api.put(`/catalogs/${catalogId}/categories/reorder`, items),
    reorderItems: (catalogId: number, items: any) =>
      api.put(`/catalogs/${catalogId}/items/reorder`, items),

    // Add to catalog operations
    addItemToCatalog: (catalogId: number, dto: any) =>
      api.post(`/catalogs/${catalogId}/items`, dto),
    addBundleToCatalog: (catalogId: number, dto: any) =>
      api.post(`/catalogs/${catalogId}/bundles`, dto),

    // Remove from catalog operations
    removeItem: (catalogId: number, itemId: number) =>
      api.delete(`/catalogs/${catalogId}/items/${itemId}`),
    removeBundle: (catalogId: number, bundleId: number) =>
      api.delete(`/catalogs/${catalogId}/bundles/${bundleId}`),

    // Update item categories
    updateItemCategories: (catalogId: number, itemId: number, dto: any) =>
      api.put(`/catalogs/${catalogId}/items/${itemId}/categories`, dto)
  }
}

// Options API (Product Customizations)
export const useOptionsApi = () => {
  const api = useApi()

  return {
    // Options CRUD
    getOptions: () => api.get('/library/options'),
    getOption: (id: number) => api.get(`/library/options/${id}`),
    createOption: (data: any) => api.post('/library/options', data),
    updateOption: (id: number, data: any) => api.put(`/library/options/${id}`, data),
    deleteOption: (id: number) => api.delete(`/library/options/${id}`)
  }
}

// Option Groups API
export const useOptionGroupsApi = () => {
  const api = useApi()

  return {
    // Option Groups CRUD
    getOptionGroups: () => api.get('/library/option-groups'),
    getOptionGroup: (id: number) => api.get(`/library/option-groups/${id}`),
    createOptionGroup: (data: any) => api.post('/library/option-groups', data),
    updateOptionGroup: (id: number, data: any) => api.put(`/library/option-groups/${id}`, data),
    deleteOptionGroup: (id: number) => api.delete(`/library/option-groups/${id}`)
  }
}
