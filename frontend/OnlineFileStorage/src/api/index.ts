

import axios from 'axios'

const api = axios.create({

  baseURL: 'http://localhost:6002/api',
  
  withCredentials: true
})
api.interceptors.response.use(
  (response) => response, 
  (error) => {
    if (error.response?.status === 401) {
      console.error('Unauthorized')

    }
    return Promise.reject(error)
  }
)

export default api