<template>
    <button @click="createChatHandler" class="create-chat-button">
      Create Chat
    </button>
  </template>
  
  <script lang="ts">
  import { defineComponent } from "vue";
  import { createChat } from "@/shared/api/chat/createChat";
  import { ChatDetails } from "@/shared/api/chat/createChat"; 
  import { useRouter } from "vue-router"; 
  
  export default defineComponent({
    props: {
      characterId: {
        type: String,
        required: true,
      },
    },
    setup(props) {
      const router = useRouter();
  
      const createChatHandler = async () => {
        try {
          const requestData = { characterId: props.characterId };
  
          const chat: ChatDetails = await createChat(requestData);
  
          router.push(`/chats/${chat.chatId}`);
        } catch (error: unknown) {
          console.error("Error creating chat:", error);
        }
      };
  
      return { createChatHandler };
    },
  });
  </script>
  
  <style scoped>
  .create-chat-button {
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