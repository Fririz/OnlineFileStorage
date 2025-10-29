<script setup lang="ts">
import { ref } from 'vue'
import api from '@/api'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router' 

const authStore = useAuthStore()
const router = useRouter()

const username = ref('')
const password = ref('')
const error = ref<string | null>(null)
defineProps<{
  loginMsg: string
}>()

const handleSubmit = () => {
  error.value = null
  api.post('/auth/login', { 
    username: username.value,
    password: password.value,
  })
  .then(response => {
    authStore.setLoggedIn(response.data)
    
    router.push('/files')
  })
  .catch(err => {
    error.value = 'Login failed!'
    console.error('Login failed:', err)
  })
}
</script>

<template>
  <div class="login-page"> 
    <h2>Login</h2>

    <form @submit.prevent="handleSubmit">
      <div>
        <label for="username">Username:</label>
        <input type="text" id="username" v-model="username" />
      </div>
      <div>
        <label for="password">Password:</label>
        <input type="password" id="password" v-model="password" />
      </div>
      <button type="submit">Login</button>
    </form>

    <p>{{ loginMsg }}</p>
  </div>
</template>

<style scoped>
.login-page {
  max-width: 400px;
  margin: 0 auto;
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
</style>