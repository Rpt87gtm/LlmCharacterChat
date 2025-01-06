<template>
    <div class="chat-details">
      <h1>{{ characterName }}</h1>
      <div class="messages">
        <div v-for="message in messages" :key="message.id" :class="`message ${message.role}`">
          {{ message.content }}
        </div>
      </div>
      <form @submit.prevent="handleSendMessage">
        <textarea v-model="newMessage" required></textarea>
        <button type="submit">Send</button>
      </form>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent, ref, onMounted } from "vue";
  import { fetchChatDetails, sendMessage } from "../api/chat/chatApi";
  import { ChatMessage, ChatDetails } from "../api/chat/chatApi";
  
  export default defineComponent({
    props: {
      chatId: {
        type: String,
        required: true,
      },
    },
    setup(props) {
      const messages = ref<ChatMessage[]>([]);
      const characterName = ref<string>("");
      const newMessage = ref<string>("");
  
      onMounted(async () => {
        try {
          const chat: ChatDetails = await fetchChatDetails(props.chatId);
          characterName.value = chat.characterName;
          messages.value = chat.messages;
        } catch (error) {
          console.error("Error loading chat details", error);
        }
      });
  
      const handleSendMessage = async () => {
        try {
            const response = await sendMessage({
            chatId: props.chatId,
            role: "user",
            content: newMessage.value,
            });

            if (response.userMessage) {
            messages.value.push(response.userMessage);
            }

            if (response.assistantMessage) {
            messages.value.push(response.assistantMessage);
            }

            newMessage.value = "";
        } catch (error) {
            console.error("Error sending message", error);
        }
        };


  
      return { messages, characterName, newMessage, handleSendMessage };
    },
  });
  </script>
  
  <style scoped>
  .chat-details {
    padding: 1rem;
  }
  .messages {
    max-height: 300px;
    overflow-y: auto;
  }
  .message {
    margin-bottom: 0.5rem;
  }
  .message.user {
    text-align: right;
  }
  </style>