import apiClient from '@/shared/api';

export const fetchMessage = () => {
  return apiClient.get('/helloWorld');
};