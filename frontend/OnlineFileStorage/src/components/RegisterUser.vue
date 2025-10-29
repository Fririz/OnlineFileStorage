<script setup lang="ts">
import { ref } from 'vue'
import api from '@/api' 

const username = ref('')
const password = ref('')

const emit = defineEmits<{
  (e: 'registerSuccess', message: string): void
  (e: 'registerFail', message: string): void
}>()

defineProps<{
  registerMsg: string
}>()

const handleSubmit = () => {
  api.post('/auth/register', { 
    username: username.value,
    password: password.value,
  })
  .then(response => {
    emit('registerSuccess', 'Registration successful!')
    console.log('Registration successful:', response.data)
  })
  .catch(error => {
    emit('registerFail', 'Registration failed! ' + error.message)
    console.error('Registration failed:', error)
  })
}
</script>

<template>
  <div class="register-page"> 
    <h2>Register</h2>

    <form @submit.prevent="handleSubmit">
      <div>
        <label for="username">Username:</label>
        <input type="text" id="username" v-model="username" />
      </div>
      <div>
        <label for="password">Password:</label>
        <input type="password" id="password" v-model="password" />
      </div>
      <button type="submit">Register</button>
    </form>

    <p>{{ registerMsg }}</p>
  </div>
</template>

<style scoped>
.register-page {
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