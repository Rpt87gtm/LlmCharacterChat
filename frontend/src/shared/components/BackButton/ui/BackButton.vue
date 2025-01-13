<template>
    <button @click="goBack" class="back-button">
      Назад
    </button>
  </template>
  
  <script lang="ts">
  import { defineComponent } from "vue";
  import { useRouter } from "vue-router";
  
  export default defineComponent({
    name: "BackButton",
    setup() {
      const router = useRouter();
  
      const goBack = () => {
        router.go(-1);
      };
  
      return {
        goBack,
      };
    },
  });
  </script>
  
  <style scoped>
  .back-button {
  position: absolute;
  top: 1rem;
  left: 1rem;
  padding: 0.5rem 1rem;
  background-color: #6C0300;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s ease;
  z-index: 10;
}
  
  .back-button:hover {
    background-color: #A60400;
  }
  </style>