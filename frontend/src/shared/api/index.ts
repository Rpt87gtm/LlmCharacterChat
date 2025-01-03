import axios from 'axios';
import { getAuthToken } from '@shared/utils/auth/auth';

const apiClient = axios.create({
  baseURL: 'http://localhost:5190/',
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config) => {
  const token = getAuthToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

export default apiClient;