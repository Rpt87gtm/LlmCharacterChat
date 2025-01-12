<template>
  <div class="container">
    <form @submit.prevent="registerUser" class="login-form">
      <h1 class="loginText">Register</h1>
      <div class="form-group">
        <input v-model="username" placeholder="Username" required class="form-input"/>
      </div>
      <div class="form-group">
        <input v-model="email" type="email" placeholder="Email" required  class="form-input"/>
      </div>
      <div class="form-group">
        <input v-model="password" type="password" placeholder="Password" required  class="form-input"/>
      </div>
      <button type="submit" class="submit-button">Register</button>
      <RouterLink to="/login" class="register-button">
        Already have an account? Login
      </RouterLink>
    </form>
  </div>
</template>

<script lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { register } from '@/features/auth/api/api';

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

<style scoped>
.container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh; /* Центрирование по вертикали */
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
  width: 300px; /* Фиксированная ширина формы */
}

.form-group {
  margin-bottom: 15px;
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

.submit-button {
  width: 100%;
  padding: 10px;
  background-color: #6C0300;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.submit-button:hover {
  background-color: #A60400;
}

.register-button {
  width: 94%;
  padding: 10px;
  margin-top: 10px;
  background-color: #6C0300;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  text-align: center;
  text-decoration: none;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.register-button:hover {
  background-color: #A60400;
}
</style>