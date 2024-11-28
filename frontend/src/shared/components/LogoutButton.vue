<template>
    <button @click="logout">Logout</button>
  </template>
  
  <script lang="ts">
  import { useStore } from 'vuex';
  import { useRouter } from 'vue-router';
  import { removeAuthToken } from '@/shared/utils/auth/auth';
  
  export default {
    setup() {
      const store = useStore();
      const router = useRouter();
  
      const logout = () => {
        store.dispatch('logout');
        removeAuthToken();
        router.push('/login');
      };
  
      return { logout };
    },
  };
  </script>