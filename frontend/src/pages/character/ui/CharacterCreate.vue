<template>
    <div>
      <h1>Create Character</h1>
      <form @submit.prevent="createNewCharacter">
        <div>
          <label for="name">Name</label>
          <input v-model="form.name" id="name" required />
        </div>
        <div>
          <label for="description">Description</label>
          <textarea v-model="form.description" id="description" required></textarea>
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
      const form = ref({ name: "", description: "" });
  
      const createNewCharacter = async () => {
        await createCharacter(form.value);
        router.push("/characters");
      };
  
      return { form, createNewCharacter };
    },
  };
  </script>
  