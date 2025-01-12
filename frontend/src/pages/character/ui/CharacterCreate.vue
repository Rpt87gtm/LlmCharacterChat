<template>
  <div class="character-create-container">
    <button @click="goBack" class="back-button">Назад</button>
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

    const goBack = () => {
      router.go(-1);
    };

    return { form, createNewCharacter, goBack };
  },
};
</script>

<style scoped>
.character-create-container {
  position: relative;
  padding: 2rem;
  width: 100%;
  margin-top: 30px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

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