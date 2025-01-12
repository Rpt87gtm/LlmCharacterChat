<script setup>
  import { ref } from 'vue'
  const count = ref(0);
</script>

<template>
    <div>
      <h1>Secret Example Page</h1>
      <p>{{ message }}</p>
    </div>
    <div class="card">
    <button type="button" @click="count++">count2 is {{ count }}</button>
    <p>
      
    </p>
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
  