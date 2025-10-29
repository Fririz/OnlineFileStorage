
import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api' 

export const useAuthStore = defineStore('auth', () => {
  const isLoggedIn = ref(false)
  const user = ref<any>(null) 

  function setLoggedIn(userData: any) {
    isLoggedIn.value = true
    user.value = userData
  }

  function setLoggedOut() {
    isLoggedIn.value = false
    user.value = null
  }
  async function checkAuthStatus() {
    try {
      const response = await api.get('auth/GetCurrentUser') 
      
      setLoggedIn(response.data)
    } catch (error) {
      setLoggedOut()
    }
  }

  return {
    isLoggedIn,
    user,
    setLoggedIn,
    setLoggedOut,
    checkAuthStatus
  }
})