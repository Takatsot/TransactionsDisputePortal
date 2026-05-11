import axios from 'axios'

// Create axios instance with base URL
const axiosInstance = axios.create({
  baseURL: 'https://localhost:44341',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add request interceptor to include auth token
axiosInstance.interceptors.request.use(
  (config) => {
    const userStr = localStorage.getItem('user')
    if (userStr) {
      try {
        const user = JSON.parse(userStr)
        if (user.token) {
          config.headers.Authorization = `Bearer ${user.token}`
        }
      } catch (error) {
        console.error('Failed to parse user from localStorage:', error)
      }
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

export default axiosInstance
