<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/api' 
import FileCard from '@/components/FileCard.vue' 
import SideMenu from '@/components/SideMenu.vue' 
import CreateItemModal from '@/components/CreateItemModel.vue'

interface ApiFileItem {
  id: string;
  name: string;
  fileSize: number; 
  type: number;
  status: number;
}

const files = ref<ApiFileItem[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const isModalOpen = ref(false)

const currentParentId = ref<string | null>(null) 

const fetchFiles = async () => {
  isLoading.value = true
  error.value = null
  files.value = []
  
  try {
    let response;
    if (currentParentId.value === null) {
      console.log('Loading from root...');
      response = await api.get('file/getitemsfromroot') 
    } else {
      console.log(`Loading children for: ${currentParentId.value}`);
      response = await api.get(`folder/getallchildren/${currentParentId.value}`)
    }
    files.value = response.data
    
  } catch (err: any) {
    if (err.response?.status === 404) {
      files.value = []
    } else {
      console.error(err)
      error.value = "Failed to load files."
    }
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchFiles()
})

const handleFileCreated = () => {
  isModalOpen.value = false 
  fetchFiles()
}

const handleOpenFolder = (folderId: string) => {
  console.log(`Opening folder: ${folderId}`)
  currentParentId.value = folderId
  fetchFiles()
}

const goBackToRoot = () => {
  console.log('Returning to root')
  currentParentId.value = null
  fetchFiles()
}

const goBack = async () => {
  if (currentParentId.value === null) return;
  
  console.log(`Requesting parent for: ${currentParentId.value}`)
  try {
    const response = await api.get(`file/getparent/${currentParentId.value}`)
    
    if (response.data && response.data.id) {
      console.log('Moving to parent:', response.data.id)
      currentParentId.value = response.data.id; 
    } else {
      console.log('Parent is root.')
      currentParentId.value = null;
    }

  } catch (err: any) {
    console.error(err)
    error.value = "Failed to load parent folder."
    currentParentId.value = null; 
  } finally {
    fetchFiles() 
  }
}

</script>

<template>
  <SideMenu 
    @refresh="fetchFiles" 
    @create-file="isModalOpen = true" 
  />
  
  <CreateItemModal 
    :isOpen="isModalOpen"
    :parentId="currentParentId" @close="isModalOpen = false"
    @created="handleFileCreated"
  />

  <main class="content-wrapper">
    <div class="controls">
      
      <template v-if="currentParentId !== null">
        <button @click="goBack" class="back-button">
          &larr; Back
        </button>
        <button @click="goBackToRoot" class="back-button root-button">
          Root
        </button>
      </template>
      
      <h2 v-else>My Files (Root)</h2>
    </div>

    <div classs="results-container">
      
      <div v-if="isLoading" class="loading-state">
        Loading...
      </div>

      <div v-else-if="error" class="error-message">
        {{ error }}
      </div>

      <div v-else-if="files.length > 0" class="file-grid">
        <FileCard
          v-for="file in files"
          :key="file.id"
          :id="file.id"         
          :name="file.name"
          :fileSize="file.fileSize"
          :fileType="file.type"
          :status="file.status"
          @open-folder="handleOpenFolder" 
        />
      </div>

      <div v-else class="empty-state">
        Folder is empty.
      </div>
    </div>
  </main>
</template>

<style scoped>
.content-wrapper {
  margin-left: 240px; 
  padding: 2rem;
  max-width: 1200px;
  
  /* --- THIS IS THE FIX --- */
  margin-top: 60px; /* (Assuming your header is 60px tall) */
  /* ----------------------- */
}
.controls {
  margin-bottom: 2rem;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.back-button {
  background: #3a3a3a;
  color: #e0e0e0;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}
.back-button:hover {
  background: #4a4a4a;
}
.root-button {
  background: #36a070;
  color: white;
}
.root-button:hover {
  background: #42b883;
}


.results-container {
  margin-top: 2rem;
}
.file-grid {
  display: flex;
  flex-wrap: wrap; 
  gap: 1.5rem;
}
.loading-state, .empty-state {
  padding: 4rem 2rem;
  text-align: center;
  color: #888;
  font-size: 1.2rem;
}
.error-message {
  padding: 1.5rem;
  text-align: center;
  color: #ffadad;
  background: #4d2424;
  border: 1px solid #8c3b3b;
  border-radius: 8px;
  font-size: 1.1rem;
}
</style>