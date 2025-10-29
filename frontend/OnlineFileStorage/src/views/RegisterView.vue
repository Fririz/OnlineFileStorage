<script setup lang="ts">
import { ref } from 'vue'
import RegisterUser from '@/components/RegisterUser.vue' 


const registerMessage = ref('') 

const handleRegisterSuccess = (message: string) => {
  registerMessage.value = message
}

const handleRegisterFail = (message: string) => {
  registerMessage.value = message
}
</script>

<template>
  <main>
    <RegisterUser
      :registerMsg="registerMessage"
      @registerSuccess="handleRegisterSuccess"
      @registerFail="handleRegisterFail"
    />
  </main>
</template>