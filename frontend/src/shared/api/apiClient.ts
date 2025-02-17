import axios, { AxiosInstance } from 'axios';
import { getAuthToken } from '@shared/utils/auth/auth';

export const SERVER_API_URL = "http://localhost:5190/api";

const apiClient: AxiosInstance = axios.create({
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