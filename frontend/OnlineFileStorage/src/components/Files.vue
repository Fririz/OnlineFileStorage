<script setup lang="ts">
import { ref } from 'vue'
import api from '@/api'


interface FileItem {
  id: number
  name: string
  size: number 
}


const files = ref<FileItem[]>([]) 
const isLoading = ref(false)
const error = ref<string | null>(null)

const hasClicked = ref(false)

const fetchYourFiles = async () => {
  hasClicked.value = true 
  isLoading.value = true
  error.value = null

  try {

    const response = await api.get('/file/getfile') 
    files.value = response.data
  } catch (err: any) {
    error.value = 'Не удалось загрузить файлы'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}


const fetchSharedFiles = () => {
  hasClicked.value = true
  isLoading.value = false
  error.value = 'Логика для общих файлов еще не готова.'
  files.value = [] 
}
</script>

<template>
  <div class="files-page">
    <button class="file-button" @click="fetchYourFiles">Your Files</button>
    <button class="file-button" @click="fetchSharedFiles">Shared Files</button>
  </div>

  <div class="results-container">
    
    <div v-if="isLoading" class="loading-state">
      Загрузка...
    </div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <ul v-else-if="files.length > 0" class="file-list">
      <li v-for="file in files" :key="file.id" class="file-item">
        {{ file.name }} ({{ file.size }} bytes)
      </li>
    </ul>
    
    <div v-else-if="hasClicked && files.length === 0 && !error" class="empty-state">
      Файлов не найдено.
    </div>
  </div>
</template>

<style scoped>

.files-page {
  max-width: 400px;
  margin: 20px;
  align-items: left;
}
.file-button {
  display: block;
  width: 100%;
  padding: 0.5rem;
  margin-bottom: 1rem;
  background-color: var(--vt-c-indigo);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.file-button:hover {
  background-color: hsla(160, 100%, 37%, 0.8);
}
div {
  margin-bottom: 1rem;
}
label {
  display: block;
  margin-bottom: 0.25rem;
}
input {
  width: 100%;
  padding: 0.5rem;
}


.results-container {
  margin-top: 2rem;
  max-width: 400px;
  margin: 20px;
}
.loading-state, .empty-state {
  padding: 1rem;
  text-align: center;
  color: #888;
}
.error-message {
  padding: 1rem;
  text-align: center;
  color: #c0392b;
  background-color: rgba(192, 57, 43, 0.1);
  border: 1px solid rgba(192, 57, 43, 0.2);
  border-radius: 4px;
}
.file-list {
  list-style: none;
  padding: 0;
}
.file-item {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 1rem;
  background-color: #fff;
  border: 1px solid #eee;
  border-radius: 4px;
  margin-bottom: 0.5rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}
</style>