<template>
    <div class="character-edit-container">
      <BackButton/>
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
  import  BackButton from "@/shared/components/BackButton/ui";
  
  export default {
    name: "CharacterEdit",
    components: {
    BackButton, 
  },
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
      return { form, handleUpdateCharacter };
    },
  };
  </script>
  
  <style scoped>
  
  .character-edit-container {
  position: relative;
  padding: 2rem;
  width: 100%;
  margin-top: 30px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  margin-bottom: 2rem;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

form div {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

label {
  font-size: 1rem;
}

input, textarea {
  padding: 0.75rem;
  border: 1px solid #444;
  border-radius: 4px;
  background-color: #fff;
  font-size: 1rem;
  transition: border-color 0.3s ease;
}

input:focus, textarea:focus {
  border-color: #A60400;
  outline: none;
}
textarea {
  resize: none;
  height: 8rem;
}
button[type="submit"] {
  padding: 0.75rem;
  background-color: #6C0300;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s ease;
}

button[type="submit"]:hover {
  background-color: #A60400;
}
  </style>