<template>
    <div>
      <h1>Example Page</h1>
      <p>{{ message }}</p>
    </div>
  </template>
  
  <script>
  import { fetchMessage } from './api';
  
  export default {
    data() {
      return {
        message: '',
      };
    },
    async created() {
      try {
        const response = await fetchMessage();
        this.message = response.data;
      } catch (error) {
        console.error('Error fetching message:', error);
      }
    },
  };
  </script>
  
  <style scoped>
  /* Add your styles here */
  </style>