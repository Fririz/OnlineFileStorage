import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api' 

interface FileItem {
  id: string;
  name: string;
  size: number;
}

export const useFileStore = defineStore('files', () => {
  const files = ref<FileItem[]>([]) 
  const isLoading = ref(false)     
  const error = ref<string | null>(null)  

  async function fetchFiles() {
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
  return {
    files,
    isLoading,
    error,
    fetchFiles
  }
})