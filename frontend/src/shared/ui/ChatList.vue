<template>
  <div class="chat-list">
    <div v-for="chat in chats" :key="chat.chatId" class="chat-item">
      <router-link :to="`/chats/${chat.chatId}`">
        <strong>{{ chat.characterName }}</strong>
        <span v-if="chat.lastMessageContent"> - {{ chat.lastMessageContent }}</span>
      </router-link>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import { fetchUserChats } from "../api/chat/fetchUserChats";
import { ChatSummary } from "../api/chat/fetchUserChats";

export default defineComponent({
  setup() {
    const chats = ref<ChatSummary[]>([]);

    onMounted(async () => {
      try {
        chats.value = await fetchUserChats();
      } catch (error) {
        console.error("Failed to fetch user chats:", error);
      }
    });

    return { chats };
  },
});
</script>

<style scoped>
.chat-list {
  padding: 1rem;
}
.chat-item {
  margin-bottom: 0.5rem;
}
</style>
