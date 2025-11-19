<script setup lang="ts">
import { computed, ref } from 'vue'
import api from '@/api'

const props = defineProps<{
  id: string
  name: string
  fileSize: number
  fileType: number // 1 = file, 2 = folder
  status: number
}>()

const emit = defineEmits<{
  (e: 'open-folder', id: string): void
  (e: 'deleted'): void
}>()

const formattedSize = computed(() => {
  if (props.status === 1) return 'Pending...'
  if (props.fileType === 2) return 'Folder'

  const bytes = props.fileSize
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`
})

const iconComponent = computed(() => {
  if (props.fileType === 2) {
    return `
      <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="folder-icon">
        <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path>
      </svg>
    `;
  } else {
    return `
      <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="file-icon-svg">
        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
        <polyline points="14 2 14 8 20 8"></polyline>
      </svg>
    `;
  }
});


const isDownloading = ref(false)
const isDeleting = ref(false)

const handleDownload = async () => {
  if (isDownloading.value || props.status === 1) return
  isDownloading.value = true

  try {
    if (props.fileType === 2) {
      return;
    }

    console.log(`Requesting download link for ID: ${props.id}`)
    const response = await api.get(`/file/downloadfile/${props.id}`)
    const downloadUrl = response.data.downloadUrl || response.data.uploadUrl

    if (!downloadUrl) {
      throw new Error('Backend did not return a download URL')
    }
    window.location.href = downloadUrl
  } catch (err) {
    console.error(err)
    alert('Failed to get download link. Check console.')
  } finally {
    isDownloading.value = false
  }
}

// --- ИЗМЕНЕННАЯ ФУНКЦИЯ ---
const handleDelete = async () => {
  if (isDeleting.value || props.status === 1) return

  if (!confirm(`Are you sure you want to delete "${props.name}"?`)) {
    return
  }

  isDeleting.value = true

  // Определяем, папка это или файл
  const isFolder = props.fileType === 2
  const itemType = isFolder ? 'folder' : 'file'

  // Выбираем URL в зависимости от типа
  const deleteUrl = isFolder
    ? `/folder/deletefolder/${props.id}` // Новый эндпоинт для папок
    : `/file/deletefile/${props.id}`   // Старый эндпоинт для файлов

  try {
    await api.delete(deleteUrl) // Используем динамический URL
    emit('deleted')
  } catch (err) {
    // Используем itemType в сообщении об ошибке
    console.error(`Failed to delete ${itemType}:`, err)
    alert(`Failed to delete ${itemType}. Check console.`)
  } finally {
    isDeleting.value = false
  }
}

</script>
<template>
  <div class="file-card" @click="fileType === 2 && status !== 1 ? emit('open-folder', id) : null" :class="{
    'folder-clickable': fileType === 2 && status !== 1,
    'is-pending': status === 1
  }">

    <div class="file-icon-wrapper">
      <div v-html="iconComponent"></div>
    </div>

    <div class="file-info">
      <h3 :title="name">{{ name }}</h3>
      <span class="file-details">{{ formattedSize }}</span>
    </div>

    <div class="file-actions">

      <button v-if="fileType === 1" class="download-btn" @click.stop="handleDownload"
        :disabled="isDownloading || status === 1">
        {{ isDownloading ? '...' : (status === 1 ? '...' : 'Download') }}
      </button>

      <div v-if="fileType === 2" class="folder-hint">
        {{ status === 1 ? '...' : '(Click to open)' }}
      </div>

      <button v-if="status !== 1" class="delete-btn" @click.stop="handleDelete" :disabled="isDeleting">
        {{ isDeleting ? '...' : 'Delete' }}
      </button>

    </div>
  </div>
</template>

<style scoped>
.folder-clickable {
  cursor: pointer;
}

.folder-clickable:hover {
  border-color: #42b883;
}

.is-pending {
  opacity: 0.5;
}

.folder-clickable.is-pending {
  cursor: not-allowed;
  border-color: #3a3a3a;
}

.folder-hint {
  font-size: 0.8rem;
  color: #888;
  margin-top: 0.75rem;
  height: 38px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.file-card {
  width: 220px;
  height: 330px;
  padding: 1.5rem;
  background: #282828;
  border: 1px solid #3a3a3a;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  transition: all 0.2s ease-in-out;
}

.file-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
}

.file-icon-wrapper {
  width: 70px;
  height: 85px;
  background-color: #3a3a3a;
  border: 1px solid #4a4a4a;
  border-radius: 8px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #42b883;
  font-size: 1.1rem;
  font-weight: 600;
  user-select: none;
  flex-shrink: 0;
}

.file-icon-wrapper::before {
  content: none;
}

.file-icon-wrapper svg {
  width: 48px;
  height: 48px;
  color: #42b883;
}

.file-info {
  margin: 1.25rem 0;
  flex-grow: 1;
  width: 100%;
  overflow: hidden;
}

.file-info h3 {
  margin: 0;
  font-size: 1.1rem;
  color: #e0e0e0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-details {
  font-size: 0.9rem;
  color: #aaaaaa;
}

.file-actions {
  width: 100%;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.download-btn {
  width: 100%;
  padding: 0.75rem;
  background-color: #42b883;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 500;
  font-size: 0.95rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
  height: 38px;
  box-sizing: border-box;
}

.download-btn:disabled {
  background-color: #555;
  color: #999;
  cursor: not-allowed;
}

.download-btn:hover:not(:disabled) {
  background-color: #36a070;
}

.delete-btn {
  width: 100%;
  padding: 0.75rem;
  background-color: #c0392b;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 500;
  font-size: 0.95rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
  height: 38px;
  box-sizing: border-box;
}

.delete-btn:disabled {
  background-color: #555;
  color: #999;
  cursor: not-allowed;
}

.delete-btn:hover:not(:disabled) {
  background-color: #a93226;
}
</style>