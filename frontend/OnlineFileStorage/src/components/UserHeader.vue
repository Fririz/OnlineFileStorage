<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ref, onMounted, onUnmounted } from 'vue'
import api from '@/api'

interface SearchItem {
  id: string
  name: string
  fileSize: number
  fileType: number // 1 = file, 2 = folder
  status: number
}

const authStore = useAuthStore()
const router = useRouter()


const searchQuery = ref('')
const searchResults = ref<SearchItem[]>([])
const isSearching = ref(false)
const showResults = ref(false)
const searchContainer = ref<HTMLElement | null>(null)
let searchTimeout: ReturnType<typeof setTimeout>

const handleSearchInput = () => {
  clearTimeout(searchTimeout)

  if (!searchQuery.value.trim()) {
    searchResults.value = []
    showResults.value = false
    return
  }

  isSearching.value = true
  showResults.value = true

  searchTimeout = setTimeout(async () => {
    try {
      // Запрос по указанному эндпоинту
      const response = await api.get<SearchItem[]>(`/search/${searchQuery.value}`)
      searchResults.value = response.data
    } catch (error) {
      console.error('Search failed:', error)
      searchResults.value = []
    } finally {
      isSearching.value = false
    }
  }, 300)
}

const goToItem = (item: SearchItem) => {
  showResults.value = false
  searchQuery.value = ''

  if (item.fileType === 2) {
    router.push(`/files/${item.id}`)
  } else {
    console.log('Open file:', item.name)
    // router.push(`/preview/${item.id}`)
  }
}

const handleClickOutside = (event: MouseEvent) => {
  if (searchContainer.value && !searchContainer.value.contains(event.target as Node)) {
    showResults.value = false
  }
}

// --- Логика выхода ---
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

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
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

    <div class="search-container" ref="searchContainer">
      <div class="input-wrapper">
        <input v-model="searchQuery" @input="handleSearchInput" @focus="searchQuery && (showResults = true)" type="text"
          placeholder="Search files..." class="search-input" />
        <span v-if="isSearching" class="loader"></span>
      </div>

      <div v-if="showResults && searchQuery" class="search-dropdown">
        <div v-if="searchResults.length === 0 && !isSearching" class="no-results">
          No items found
        </div>

        <ul v-else class="results-list">
          <li v-for="item in searchResults" :key="item.id" @click="goToItem(item)" class="result-item">
            <span class="item-icon">{{ item.fileType === 2 ? '📁' : '📄' }}</span>
            <div class="item-info">
              <span class="item-name">{{ item.name }}</span>
            </div>
          </li>
        </ul>
      </div>
    </div>

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
  z-index: 2;
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
  z-index: 2;
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

.search-container {
  position: relative;
  margin-left: auto;
  margin-right: 2rem;
  width: 300px;
}

.input-wrapper {
  position: relative;
  width: 100%;
}

.search-input {
  width: 100%;
  padding: 0.5rem 1rem;
  padding-right: 2rem;
  border-radius: 6px;
  border: 1px solid #3a3a3a;
  background-color: #2a2a2a;
  color: white;
  outline: none;
  transition: border-color 0.2s;
}

.search-input:focus {
  border-color: #42b883;
}

.loader {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  width: 14px;
  height: 14px;
  border: 2px solid #555;
  border-top-color: #42b883;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: translateY(-50%) rotate(360deg);
  }
}

.search-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  width: 100%;
  background-color: #2a2a2a;
  border: 1px solid #3a3a3a;
  border-radius: 0 0 6px 6px;
  margin-top: 4px;
  max-height: 300px;
  overflow-y: auto;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
  z-index: 100;
}

.results-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.result-item {
  display: flex;
  align-items: center;
  padding: 0.75rem 1rem;
  cursor: pointer;
  border-bottom: 1px solid #333;
  transition: background 0.2s;
}

.result-item:last-child {
  border-bottom: none;
}

.result-item:hover {
  background-color: #3a3a3a;
}

.item-icon {
  margin-right: 0.75rem;
  font-size: 1.2rem;
}

.item-info {
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.item-name {
  font-size: 0.9rem;
  color: #e0e0e0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.no-results {
  padding: 1rem;
  text-align: center;
  color: #888;
  font-size: 0.9rem;
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