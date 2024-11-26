import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5190/',
  headers: {
    'Content-Type': 'application/json',
  },
});

export default apiClient;