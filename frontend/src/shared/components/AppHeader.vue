<template>
  <header class="header">
      <div id="header-conent">
        <h1>LLM Chat</h1>
          <div class="left-section">
              <RouterLink to="/characters" class="button" v-if="showHeader">Characters</RouterLink>
          </div>
          <div class="right-section">
              <LogoutButton class="logout-button" v-if="showHeader" />
          </div>
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
    height: 152px;
  }
  .logout-button {
    
    margin-right: 2rem; 
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
  height: 120px;
}
.button {
    border-radius: 8px;
    border: 1px solid transparent;
    padding: 0.6em 1.2em;
    
    margin-left: 2rem; 
    font-size: 1em;
    font-weight: 500;
    font-family: inherit;
    background-color: #6C0300;
    cursor: pointer;
    transition: border-color 0.25s, background-color 0.25s, color 0.25s;
    color: #A60400;
    text-decoration: none;
}
.button:hover {
  background-color: #A60400;
  color: #6C0300;
}
.left-section {
    display: flex;
    align-items: center;
    margin-right: auto; 
}
  </style>