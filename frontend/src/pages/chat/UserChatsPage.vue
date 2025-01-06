<template>
    <div class="chats-page">
      <h1>Your Chats</h1>
      <ChatList />
    </div>
  </template>
  
  <script>
  import ChatList from '@/shared/ui/ChatList.vue';
  
  export default {
    components: { ChatList },
  };
  </script>
  