<template>
    <header class="header">
        <div id="header-conent">
            <h1>LLM Chat</h1>
            <LogoutButton  class="logout-button" v-if="showHeader" />   
        </div>
    </header>
  </template>
  
  <script lang="ts">
  import LogoutButton from './LogoutButton.vue';
  import { useRoute } from 'vue-router';
  import { computed } from 'vue';
  
  export default {
    components: { LogoutButton },
    setup() {
        const route = useRoute();

        const showHeader = computed(() => {
        const excludedRoutes = ['/login', '/register'];
        return !excludedRoutes.includes(route.path);
        });

        return { showHeader };
    },
  };
  </script>
  
  <style scoped>
  .header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    background-color: #D3625F; 
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    z-index: 1000;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    width: 100%;
  }
  .logout-button {
    
    margin-right: 2rem; /* Добавляем отступ справа */
}
#header-conent {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  margin: 2rem;
  margin-bottom: 0rem;
  padding-left: 2rem;
  padding-right: 2rem;
  color: #7D221F; 
  background-color: #D33935; 
}
  </style>