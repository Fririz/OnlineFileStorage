<script setup lang="ts">
// Вся логика для логаута теперь живет ЗДЕСЬ
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
  padding: 1rem 2rem;
  background-color: var(--vt-c-indigo);
  color: white;
  width: 100%;
  
  position: relative; 
}
.logo {
  font-size: 1.5rem;
  font-weight: bold;
  color: white;
  text-decoration: none;
  transition: color 0.2s ease;
}
.logo:hover {
  color: #bdc3c7;
}

.main-navigation {
  display: flex;
  gap: 1.5rem; 
  position: absolute;  
  left: 17%;           
  transform: translateX(-50%); 
}

.main-navigation a {
  color: #bdc3c7; 
  text-decoration: none;
  font-size: 1rem;
  padding: 0.25rem 0;
  border-bottom: 2px solid transparent; 
  border-radius: 20%;
  transition: all 0.2s ease;
}

.main-navigation a:hover {
  color: white; 
  
}


.main-navigation a.router-link-exact-active {

  color: var(--vt-c-green); 
  font-weight: bold;
  border-bottom-color: var(--vt-c-green);
}

.user-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}


.logout-button:hover {
  background-color: #c0392b;
}
</style>