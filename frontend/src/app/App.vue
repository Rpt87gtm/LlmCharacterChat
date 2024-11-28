
<template>
  <div id="app">
    <AppHeader />
    <router-view />
  </div>
</template>



<script lang="ts">
import AppHeader from '@shared/components/AppHeader.vue';

export default {
  components: { AppHeader },
  name: 'App',
};
</script>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}

</style>
