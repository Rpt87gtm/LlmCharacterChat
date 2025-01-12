<template>
    <div class="chats-page">
      <BackButton />
      <h1>Your Chats</h1>
      <ChatList />
    </div>
  </template>
  
  <script>
  import ChatList from '@/pages/chat/chatsPage/ui/ChatList.vue';
  import BackButton from '@/shared/components/BackButton/ui'; 

  export default {
    components: { 
      ChatList,
      BackButton,
    },
  };
  </script>
  <style scoped>
  .chats-page {
    padding: 1rem;
    width: 100%;
    position: relative; 
  }
  
  h1 {
    text-align: center;
    margin-bottom: 1rem;
  }
  </style>