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

      <!-- Кнопка для создания чата -->
      <button @click="createChatHandler" class="create-chat-button">
        Create Chat
      </button>
    </div>
  </div>
</template>

<script lang="ts">
import { fetchCharacterById, deleteCharacter } from "@/shared/api/character";
import { createChat } from "@/shared/api/chat/createChat";
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

    // Обработчик для создания чата
    const createChatHandler = async () => {
      try {
        const requestData = { characterId: route.params.id as string }; // Используем characterId из route
        const chat = await createChat(requestData); // Создаем чат
        router.push(`/chats/${chat.chatId}`); // Переходим на страницу чата
      } catch (error) {
        console.error("Error creating chat:", error);
      }
    };

    return {
      character,
      isCreator,
      editCharacter,
      handleDeleteCharacter,
      goBack,
      createChatHandler, // Возвращаем метод для использования в шаблоне
    };
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
  background-color: #6c0300;
  color: #a60400;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s ease;
  z-index: 10;
}

.back-button:hover {
  background-color: #a60400;
  color: #6c0300;
}

.character-details-content {
  margin-top: 4rem;
  width: 100%;
  max-width: none;
  padding: 0 1rem;
}

.create-chat-button {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.create-chat-button:hover {
  background-color: #0056b3;
}
</style>