<template>
    <div class="chat-page">
      <ChatDetails :chatId="$route.params.chatId" />
    </div>
  </template>
  
  <script>
  import ChatDetails from '@/pages/chat/chatPage/ui/ChatDetails.vue';
  
  export default {
    components: { ChatDetails },
  };
  </script>
  