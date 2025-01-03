<template>
    <div>
      <h1>{{ character.name }}</h1>
      <p>{{ character.description }}</p>
      <button @click="deleteCharacter">Delete</button>
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
      const character = ref(null);
  
      onMounted(async () => {
        character.value = await fetchCharacterById(route.params.id as string);
      });
  
      const deleteCharacter = async () => {
        await deleteCharacter(route.params.id as string);
        router.push("/characters");
      };
  
      return { character, deleteCharacter };
    },
  };
  </script>
  