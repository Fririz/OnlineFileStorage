<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useAuthStore } from './stores/auth'
import WelcomeText from './components/WelcomeText.vue';
import UserHeader from './components/UserHeader.vue'; 


const authStore = useAuthStore()

</script>

<template>
  <header>
      <div v-if="!authStore.isLoggedIn" class="nav-guest">
        <WelcomeText msg="Welcome to the File Storage App!" />
        <nav>
          <RouterLink to="/register">Register</RouterLink>
          <RouterLink to="/login">Login</RouterLink>
          <RouterLink to="/about">About Us</RouterLink>
        </nav>
      </div>

      <UserHeader v-if="authStore.isLoggedIn" />

  </header>

  <RouterView />
</template>

<style scoped>

.nav-guest{
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem 0;
}

header {
  line-height: 1.5;
  width: 100%;
}

nav {
  width: 100%;
  font-size: 12px;
  text-align: center;
  margin-top: 2rem;
}

nav a.router-link-exact-active {
  color: var(--color-text);
}

nav a.router-link-exact-active:hover {
  background-color: transparent;
}

nav a {
  display: inline-block;
  padding: 0 1rem;
  border-left: 1px solid var(--color-border);
}

nav a:first-of-type {
  border: 0;
}


@media (min-width: 1024px) {
  header {
  }

  nav {

  }
}
</style>