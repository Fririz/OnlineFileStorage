<script setup lang="ts">
import { ref, watch } from 'vue'
import api from '@/api'
import axios from 'axios'

const props = defineProps<{
  isOpen: boolean
  parentId: string | null
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'created'): void
}>()

const itemType = ref<1 | 2>(2)
const itemName = ref('')
const selectedFile = ref<File | null>(null)
const isLoading = ref(false)
const error = ref<string | null>(null)

watch(itemType, () => {
  itemName.value = ''
  selectedFile.value = null
  error.value = null
})

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    itemType.value = 2
    itemName.value = ''
    selectedFile.value = null
    isLoading.value = false
    error.value = null
  }
})

const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    const file = target.files[0]
    if (file) {
      selectedFile.value = file
    } else {
      selectedFile.value = null
    }
  } else {
    selectedFile.value = null
  }
}

const handleSubmit = async () => {
  isLoading.value = true
  error.value = null

  try {
    let dto: { Name: string; ParentId: string | null; Type: 1 | 2 }

    if (itemType.value === 2) {
      if (!itemName.value) {
        throw new Error('Folder name is required')
      }
      dto = {
        Name: itemName.value,
        ParentId: props.parentId,
        Type: 2,
      }
      await api.post('folder/createfolder', dto)
    } else {
      if (!selectedFile.value) {
        throw new Error('File is required')
      }

      dto = {
        Name: selectedFile.value.name,
        ParentId: props.parentId,
        Type: 1,
      }

      console.log('Шаг 1: Отправка DTO на /api/file/uploadfile', dto)
      const response = await api.post('file/uploadfile', dto)

      const uploadUrl = response.data.uploadUrl

      if (!uploadUrl || typeof uploadUrl !== 'string') {
        throw new Error('Backend did not return "uploadUrl" in response')
      }

      console.log('Received link, uploading file to:', uploadUrl)

      await axios.post(
        uploadUrl,
        selectedFile.value,
        {
          withCredentials: true,
          headers: {
            'Content-Type': selectedFile.value.type || 'application/octet-stream'
          }
        }
      )

      console.log('File uploaded successfully.')
    }

    isLoading.value = false
    emit('created')
    emit('close')

  } catch (err: any) {
    console.error(err)
    error.value = err.response?.data?.message || err.message || 'An error occurred'
    isLoading.value = false
  }
}
</script>

<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="emit('close')">
    <div class="modal-content">
      <h3>Create New</h3>

      <div class="type-selector">
        <label>
          <input type="radio" v-model="itemType" :value="2" />
          Folder
        </label>
        <label>
          <input type="radio" v-model="itemType" :value="1" />
          File
        </label>
      </div>

      <form @submit.prevent="handleSubmit">
        <div v-if="itemType === 2" class="form-group">
          <label for="folderName">Folder Name</label>
          <input id="folderName" type="text" v-model="itemName" placeholder="New Folder" />
        </div>

        <div v-if="itemType === 1" class="form-group">
          <label for="fileUpload">Choose file</label>
          <input id="fileUpload" type="file" @change="handleFileChange" />
        </div>

        <div v-if="error" class="error-message">
          {{ error }}
        </div>

        <div class="modal-actions">
          <button type="button" class="btn-cancel" @click="emit('close')">
            Cancel
          </button>
          <button type="submit" class="btn-create" :disabled="isLoading">
            {{ isLoading ? 'Loading...' : (itemType === 2 ? 'Create' : 'Upload') }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}

.modal-content {
  background: #282828;
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  border: 1px solid #3a3a3a;
}

h3 {
  margin-top: 0;
  text-align: center;
  color: #ffffff;
  font-weight: 600;
}

.type-selector {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
  border: 1px solid #4a4a4a;
  border-radius: 8px;
  overflow: hidden;
}

.type-selector label {
  flex: 1;
  padding: 0.75rem;
  text-align: center;
  cursor: pointer;
  background: #3a3a3a;
  color: #aaaaaa;
  transition: all 0.2s ease;
}

.type-selector input {
  display: none;
}

.type-selector label:has(input:checked) {
  background: #42b883;
  color: white;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #e0e0e0;
}

.form-group input[type="text"],
.form-group input[type="file"] {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #4a4a4a;
  border-radius: 8px;
  box-sizing: border-box;
  background: #3a3a3a;
  color: #e0e0e0;
}

.form-group input[type="file"] {
  color: #aaaaaa;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}

.btn-cancel,
.btn-create {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-cancel {
  background: #4a4a4a;
  color: #e0e0e0;
}

.btn-cancel:hover {
  background: #5a5a5a;
}

.btn-create {
  background: #42b883;
  color: white;
}

.btn-create:hover {
  background: #36a070;
}

.btn-create:disabled {
  background: #555;
  color: #999;
  cursor: not-allowed;
}

.error-message {
  color: #ffadad;
  background: #4d2424;
  border: 1px solid #8c3b3b;
  padding: 0.75rem;
  border-radius: 8px;
  margin-top: 1rem;
  text-align: center;
}
</style>