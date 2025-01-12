<template>
  <div class="chat-details">
    <h1>{{ chatSocketState.characterName }}</h1>
    <div v-if="chatSocketState.error" class="error">
      {{ chatSocketState.error }}
    </div>
    
    <div class="messages">
      <div
        v-for="message in chatSocketState.messages"
        :key="message.Id"
        :class="`message ${message.Role}`"
      >
        {{ message.Content }}
      </div>
    </div>
    <form @submit.prevent="handleSendMessage">
      <textarea v-model="newMessage" required :disabled="!chatSocketState.isConnected"></textarea>
      <button type="submit" :disabled="!chatSocketState.isConnected">Send</button>
    </form>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, onUnmounted } from "vue";
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

    const onMessageCallback = (response: any) => {
      if (response.assistantMessage) {
        console.log("Assistant responded:", response.assistantMessage);
      }
    };

    onMounted(() => {
      disconnectWebSocket(); 
      connectWebSocket(props.chatId, onMessageCallback); 
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

    return {
      newMessage,
      chatSocketState,
      handleSendMessage,
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
</style>