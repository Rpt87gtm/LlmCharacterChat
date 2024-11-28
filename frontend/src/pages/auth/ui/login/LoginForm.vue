<template>
  <form @submit.prevent="loginUser">
    <input v-model="username" placeholder="Username" required />
    <input v-model="password" type="password" placeholder="Password" required />
    <button type="submit">Login</button>
  </form>
</template>

<script lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { login } from '@/features/auth/api/api'
import { setAuthToken } from '@/shared/utils/auth/auth';

export default {
  setup() {
    const router = useRouter();
    const username = ref('');
    const password = ref('');

    const loginUser = async () => {
      try {
        const userData = {
          username: username.value,
          password: password.value,
        };
        const response = await login(userData);
        console.log('Login successful:', response);
        setAuthToken(response.token); // Сохраняем токен
        router.push('/');
      } catch (error) {
        console.error('Login failed:', error);
      }
    };

    return { username, password, loginUser };
  },
};
</script>