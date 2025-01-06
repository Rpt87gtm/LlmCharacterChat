<template>
  <div class="character-details-container">
    <button @click="goBack" class="back-button">Назад</button>
    <div class="character-details-content">
      <h1>{{ character.name }}</h1>
      <p>{{ character.systemPrompt }}</p>
      <p>Создатель: {{ character.createdByAppUserName }}</p>

      <!-- Кнопки отображаются только для создателя -->
      <div v-if="isCreator">
        <button @click="editCharacter">Edit</button>
        <button @click="handleDeleteCharacter">Delete</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { fetchCharacterById, deleteCharacter } from "@/shared/api/character";
import { useRoute, useRouter } from "vue-router";
import { ref, onMounted, computed } from "vue";
import { getUserIdFromToken } from "@/shared/utils/auth/auth";

export default {
  name: "CharacterDetails",
  setup() {
    const route = useRoute();
    const router = useRouter();
    const currentUserId = ref(getUserIdFromToken()); 
    const character = ref({
      name: "",
      systemPrompt: "",
      createdByAppUserId: "",
      createdByAppUserName: "",
    });

    

    onMounted(async () => {
      const fetchedCharacter = await fetchCharacterById(route.params.id as string);
      character.value = fetchedCharacter;
    });

    const isCreator = computed(() => {
      return character.value.createdByAppUserId === currentUserId.value;
    });

    const editCharacter = () => {
      router.push(`/characters/edit/${route.params.id}`);
    };

    const handleDeleteCharacter = async () => {
      await deleteCharacter(route.params.id as string);
      router.push("/characters");
    };

    const goBack = () => {
      router.go(-1);
    };

    return { character, isCreator, editCharacter, handleDeleteCharacter, goBack };
  },
};
</script>


<style scoped>
.character-details-container {
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

.character-details-content {
  margin-top: 4rem;
  width: 100%;
  max-width: none;
  padding: 0 1rem;
}
</style>