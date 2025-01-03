<template>
    <div>
      <h1>Characters</h1>
      <router-link to="/characters/create">Create Character</router-link>
      <ul>
        <li v-for="character in characters" :key="character.id">
          <router-link :to="`/characters/${character.id}`">{{ character.name }}</router-link>
        </li>
      </ul>
    </div> 
  </template>
  
  <script lang="ts">
  import { fetchCharacters } from "@/shared/api/character";
  import { ref, onMounted } from "vue";
  
  export default {
    name: "CharacterList",
    setup() {  
      const characters = ref([]);
      
      onMounted(async () => {
        characters.value = await fetchCharacters();
      });
  
      return { characters };
    },
  };
  </script>
  