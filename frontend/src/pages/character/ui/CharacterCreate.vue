<template>
    <div>
      <h1>Create Character</h1>
      <form @submit.prevent="createNewCharacter">
        <div>
          <label for="Name">Name</label>
          <input v-model="form.Name" id="Name" required />
        </div>
        <div>
          <label for="SystemPrompt">SystemPrompt</label>
          <textarea v-model="form.SystemPrompt" id="SystemPrompt" required></textarea>
        </div>
        <button type="submit">Create</button>
      </form>
    </div>
  </template>
  
  <script lang="ts">
  import { createCharacter } from "@/shared/api/character";
  import { ref } from "vue";
  import { useRouter } from "vue-router";
  
  export default {
    name: "CharacterCreate",
    setup() {
      const router = useRouter();
      const form = ref({ Name: "", SystemPrompt: "" });
  
      const createNewCharacter = async () => {
        await createCharacter(form.value);
        router.push("/characters");
      };
  
      return { form, createNewCharacter };
    },
  };
  </script>
  