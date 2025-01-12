<template>
  <div class="chat-details">
    <h1>{{ chatSocketState.characterName }}</h1>
    <div v-if="chatSocketState.error" class="error">
      {{ chatSocketState.error }}
    </div>
    
    <div class="messages" ref="messagesContainer">
      <div
        v-for="message in chatSocketState.messages"
        :key="message.Id"
        :class="`message ${message.Role}`"
      >
        {{ message.Content }}
      </div>
    </div>
    <form @submit.prevent="handleSendMessage" class="chat-form">
      <textarea
        v-model="newMessage"
        required
        :disabled="!chatSocketState.isConnected"
        placeholder="Type your message..."
      ></textarea>
      <button type="submit" :disabled="!chatSocketState.isConnected">Send</button>
    </form>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, onUnmounted, nextTick } from "vue";
import { connectWebSocket, sendSocketMessage, chatSocketState, ChatMessage, disconnectWebSocket } from "@shared/api/chat/chatApi";

export default defineComponent({
  props: {
    chatId: {
      type: String,
      required: true,
    },
  },
  setup(props) {
    const newMessage = ref<string>("");
    const messagesContainer = ref<HTMLElement | null>(null); // Ссылка на контейнер сообщений

    const onMessageCallback = (response: any) => {
      if (response.AssistantMessage || response.UserMessage) {
        nextTick(() => scrollToBottom());
      } 
    };
    const onLoading = () =>{
      nextTick(() => scrollToBottom()); 
    }

    onMounted(() => {
      disconnectWebSocket(); 
      connectWebSocket(props.chatId, onMessageCallback, onLoading); 
      
    });
    

    onUnmounted(() => {
      disconnectWebSocket(); 
    });

    const handleSendMessage = () => {
      if (newMessage.value.trim()) {
        const message: ChatMessage = {
          ChatHistoryId: props.chatId,
          Role: "user",
          Content: newMessage.value.trim(),
        };
        sendSocketMessage(message);
        newMessage.value = "";
        
      }
    };

    const scrollToBottom = () => { 
      if (messagesContainer.value) {
        messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
      }
    };

    return {
      newMessage,
      chatSocketState,
      handleSendMessage,
      messagesContainer,
    };
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
.message.assistant {
  text-align: left;
}
.error {
  color: red;
  margin-bottom: 1rem;
}
textarea {
  flex: 1;
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 1rem;
  resize: none; 
  transition: border-color 0.3s ease;
}
.chat-form {
  display: flex;
  gap: 0.5rem;
  padding: 0.3rem;
  background-color: #6C0300;
  border-radius: 8px;
}

button {
  padding: 0.75rem 1.5rem;
  background-color: #d33935;
  color: #6C0300;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s ease;
}
button:hover:not(:disabled) {
  background-color: #A60400;
}
</style>