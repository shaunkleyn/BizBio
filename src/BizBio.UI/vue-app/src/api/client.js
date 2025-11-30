import axios from 'axios'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer 3a97a099de051807b89d876cf1fcdce0',
    'CF-Access-Client-Id': '5c8e5c7bdf1bb4d019f52d494744531a.access',
    'CF-Access-Client-Secret': 'a3c1954f4f35b7265abf9b89ba4b0a3f03b2f07b303679b6264beabea5d92274',
    'Access-Control-Allow-Origin': '*'
  },
  withCredentials: false,

})

// Add request interceptor to include JWT token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
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
    // 2. It's NOT from the login endpoint (user trying to login)
    // 3. It's NOT from the register endpoint
    const isAuthEndpoint = error.config?.url?.includes('/auth/login') ||
                          error.config?.url?.includes('/auth/register')

    if (error.response?.status === 401 && !isAuthEndpoint) {
      // Token expired or invalid, redirect to login
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default apiClient
