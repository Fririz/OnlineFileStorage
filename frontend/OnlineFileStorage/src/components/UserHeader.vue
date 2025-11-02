<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import api from '@/api'

const authStore = useAuthStore()
const router = useRouter()

const handleLogout = async () => {
  try {
    await api.get('/auth/logout')
  } catch (error) {
    console.error('Logout request failed:', error)
  } finally {
    authStore.setLoggedOut()
    router.push('/login')
  }
}
</script>

<template>
  <header class="nav-user-header">
    
    <RouterLink to="/" class="logo">
      FileStorage
    </RouterLink>

    <nav class="main-navigation">
      <RouterLink to="/files">My Files</RouterLink>
      <RouterLink to="/profile">Profile</RouterLink>
    </nav>

    <div class="user-controls">
      <button @click="handleLogout" class="logout-button">Logout</button>
    </div>

  </header>
</template>

<style scoped>
.nav-user-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 2rem;
  background-color: #1a1a1a;
  color: #e0e0e0;
  width: 100%;
  border-bottom: 1px solid #3a3a3a;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 20;
  height: 60px;
  box-sizing: border-box;
}
.logo {
  font-size: 1.5rem;
  font-weight: bold;
  color: #e0e0e0;
  text-decoration: none;
  transition: color 0.2s ease;
}
.logo:hover {
  color: #ffffff;
}

.main-navigation {
  display: flex;
  gap: 1.5rem; 
  position: absolute;  
  left: 17%;           
  transform: translateX(-50%); 
}

.main-navigation a {
  color: #aaaaaa; 
  text-decoration: none;
  font-size: 1rem;
  padding: 0.25rem 0;
  border-bottom: 2px solid transparent; 
  transition: all 0.2s ease;
}

.main-navigation a:hover {
  color: white; 
}

.main-navigation a.router-link-exact-active {
  color: #42b883; 
  font-weight: bold;
  border-bottom-color: #42b883;
}

.user-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.logout-button {
  background-color: #3a3a3a;
  color: #e0e0e0;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s ease;
}

.logout-button:hover {
  background-color: #c0392b;
  color: white;
}
</style>