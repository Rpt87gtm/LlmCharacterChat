<template>
  <div class="container">
    
    <form @submit.prevent="loginUser" class="login-form">
      <h1 class="loginText">Login</h1>
      <div class="form-group">
        <input v-model="username" placeholder="Username" required class="form-input" />
      </div>
      <div class="form-group">
        <input v-model="password" type="password" placeholder="Password" required class="form-input" />
      </div>
      <button type="submit" class="submit-button">Login</button>
    </form>
  </div>
</template>

<script lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { login } from '@/features/auth/api/api';
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
        setAuthToken(response.token); 
        router.push('/');
      } catch (error) {
        console.error('Login failed:', error);
      }
    };

    return { username, password, loginUser };
  },
};
</script>

<style scoped>
.container {
  display: flex;
  justify-content: center;
  align-items: center;
}

.login-form {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 30px;
  border: 1px solid #ccc;
  border-radius: 8px;
  background-color: #f9f9f9;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.form-group {
  margin-bottom: 15px;
  margin-right: 20px;
  width: 100%;
}

.form-input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 16px;
}
.loginText {
  margin-top: 0px;
  color:#D33935;
  font-family: 'Inter', sans-serif;
  font-weight: 300; 
}
</style>