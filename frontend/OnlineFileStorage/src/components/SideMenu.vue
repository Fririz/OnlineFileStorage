<script setup lang="ts">
const emit = defineEmits<{
  (e: 'refresh'): void
  (e: 'create-file'): void
}>()
</script>

<template>
  <aside class="side-menu">
    <h3>My Drive</h3>

    <button class="menu-button create-btn" @click="emit('create-file')">
      <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
      Create
    </button>
    
    <button class="menu-button refresh-btn" @click="emit('refresh')">
      <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M23 4v6h-6"></path><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"></path></svg>
      Refresh
    </button>

  </aside>
</template>

<style scoped>
.side-menu {
  position: fixed;
  top: 60px; 
  left: 0;
  height: calc(100vh - 60px); 
  width: 240px;
  background: #1a1a1a;
  border-right: 1px solid #3a3a3a;
  padding: 1.5rem;
  z-index: 10; 
}

h3 {
  margin-top: 0;
  margin-bottom: 2rem;
  color: #ffffff;
  font-weight: 600;
}

.menu-button {
  width: 100%;
  padding: 0.75rem 1rem;
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.95rem;
  font-weight: 500;
  text-align: left;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.menu-button svg {
  flex-shrink: 0;
  color: #e0e0e0;
}

.create-btn {
  background-color: #42b883;
  color: white;
}
.create-btn:hover {
  background-color: #36a070;
}
.create-btn svg {
  color: white;
}

.refresh-btn {
  background-color: #3a3a3a;
  color: #e0e0e0;
}
.refresh-btn:hover {
  background-color: #4a4a4a;
}
</style>