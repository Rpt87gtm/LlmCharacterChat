<template>
  <div class="character-details-container">
    <BackButton/>
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
import  BackButton from "@/shared/components/BackButton/ui";

export default {
  name: "CharacterDetails",
  components: {
    BackButton, 
  },
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


    const createChatHandler = async () => {
      try {
        const requestData = { characterId: route.params.id as string }; 
        const chat = await createChat(requestData); 
        router.push(`/chats/${chat.chatId}`); 
      } catch (error) {
        console.error("Error creating chat:", error);
      }
    };

    return {
      character,
      isCreator,
      editCharacter,
      handleDeleteCharacter,
      createChatHandler, 
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



.character-details-content {
  margin-top: 4rem;
  width: 100%;
  max-width: none;
  padding: 0 1rem;
}

.create-chat-button {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #6C0300;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.create-chat-button:hover {
  background-color: #D33935;
  color:#000;
}
</style>