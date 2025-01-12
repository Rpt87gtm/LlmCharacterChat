<template>
    <div class="chat-page">
      <BackButton/>
      <ChatDetails :chatId="$route.params.chatId" />
    </div>
  </template>
  
  <script>
  import ChatDetails from '@/pages/chat/chatPage/ui/ChatDetails.vue';
  import BackButton from '@/shared/components/BackButton/ui'; 

  export default { 
    components: {
       ChatDetails,
       BackButton,
      },
  };
  </script>
  <style scoped>
  .chat-page {
    padding: 1rem;
    width: 100%;
    position: relative; 
  }
  
  h1 {
    text-align: center;
    margin-bottom: 1rem;
  }
  </style>