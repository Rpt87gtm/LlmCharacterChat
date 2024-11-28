<template>
    <header>
      <h1>My App</h1>
      <LogoutButton />
    </header>
  </template>
  
  <script lang="ts">
  import LogoutButton from './LogoutButton.vue';
  
  export default {
    components: { LogoutButton },
  };
  </script>
  
  <style scoped>
  header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    background-color: #f0f0f0;
  }
  </style>