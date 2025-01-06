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
  color: #6C0300;
}
</style>