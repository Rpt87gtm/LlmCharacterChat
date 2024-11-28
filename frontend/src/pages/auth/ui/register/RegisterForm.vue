<template>
    <form @submit.prevent="register">
      <input v-model="username" placeholder="Username" required />
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit">Register</button>
    </form>
  </template>
  
  <script>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { register } from '@/features/auth/api';
  
  export default {
    setup() {
      const router = useRouter();
      const username = ref('');
      const email = ref('');
      const password = ref('');
  
      const registerUser = async () => {
        try {
          const userData = {
            username: username.value,
            email: email.value,
            password: password.value,
          };
          await register(userData);
          router.push('/');
        } catch (error) {
          console.error(error);
        }
      };
   
      return { username, email, password, registerUser };
    },
  };
  </script>