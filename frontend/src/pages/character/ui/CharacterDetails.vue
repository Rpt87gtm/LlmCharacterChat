<template>
    <div>
      <h1>{{ character.name }}</h1>
      <p>{{ character.systemPrompt }}</p>
      <button @click="handleDeleteCharacter">Delete</button>
    </div>
  </template>
  
  <script lang="ts">
  import { fetchCharacterById, deleteCharacter } from "@/shared/api/character";
  import { useRoute, useRouter } from "vue-router";
  import { ref, onMounted } from "vue";
  
  export default {
    name: "CharacterDetails",
    setup() {
      const route = useRoute();
      const router = useRouter();
      const character = ref({ name: '', systemPrompt: '' });
  
      onMounted(async () => {
        character.value = await fetchCharacterById(route.params.id as string);
      });
  
      const handleDeleteCharacter  = async () => {
        await deleteCharacter(route.params.id as string);
        router.push("/characters");
      };
  
      return { character, handleDeleteCharacter };
    },
  };
  </script>
  