<template>
    <div id="loginForm">
      
      <LoginForm />
    </div>
  </template>
  
  <script>
  import LoginForm from './LoginForm.vue';
  
  export default {
    components: { LoginForm },
  };
  </script>

<style scoped>
.loginForm{
  justify-content: center;
  align-items: center;
}
</style>