<template>
    <div class="character-edit-container">
      <button @click="goBack" class="back-button">Назад</button>
      <h1>Edit Character</h1>
      <form @submit.prevent="handleUpdateCharacter">
        <div>
          <label for="Name">Name</label>
          <input v-model="form.Name" id="Name" required />
        </div>
        <div>
          <label for="SystemPrompt">System Prompt</label>
          <textarea v-model="form.SystemPrompt" id="SystemPrompt" required></textarea>
        </div>
        <button type="submit">Update</button>
      </form>
    </div>
  </template>
  
  <script lang="ts">
  import { ref, onMounted } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import { fetchCharacterById, updateCharacter } from "@/shared/api/character";
  
  export default {
    name: "CharacterEdit",
    setup() {
      const route = useRoute();
      const router = useRouter();
      const form = ref({ Name: "", SystemPrompt: "" });
  
      onMounted(async () => {
        if (route.params.id) {
          const character = await fetchCharacterById(route.params.id as string);
          form.value = { Name: character.name, SystemPrompt: character.systemPrompt };
        }
      });
  
      const handleUpdateCharacter = async () => {
        if (route.params.id) {
          await updateCharacter(route.params.id as string, form.value);
          router.push("/characters"); 
        } else {
          console.error("ID is missing");
        }
      };
  
      const goBack = () => {
        router.go(-1);
      };
  
      return { form, handleUpdateCharacter, goBack };
    },
  };
  </script>
  
  <style scoped>
  .character-edit-container {
    position: relative;
    padding: 2rem;
    width: 100%;
    max-width: none;
  }
  
  .back-button {
    position: absolute;
    top: 1rem;
    left: 1rem;
    padding: 0.5rem 1rem;
    background-color: #6C0300;
    color: #A60400;
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