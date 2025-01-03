
<template>
  <div id="app">
    <AppHeader v-if="showHeader" />
    <div class="main-content">
      <router-view />
    </div>
  </div>
</template>



<script lang="ts">
import AppHeader from '@shared/components/AppHeader.vue';
import { useRoute } from 'vue-router';
import { computed } from 'vue';

export default {
  components: { AppHeader },
  name: 'App',
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
#app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  padding-top: 60px; /* Высота хедера + отступ */
  width: 100%;
}
</style>
