<script setup lang="ts">
import { ref } from 'vue'
import LoginUser from '@/components/LoginUser.vue' 


const loginMessage = ref('') 

const handleLoginSuccess = (message: string) => {
  loginMessage.value = message
}

const handleLoginFail = (message: string) => {
  loginMessage.value = message
}
</script>

<template>
  <main>
    <LoginUser 
      :loginMsg="loginMessage"
      @loginSuccess="handleLoginSuccess"
      @loginFail="handleLoginFail"
    />
  </main>
</template>