import axios from 'axios';

const API_URL = 'http://localhost:5190/api/account';

export const register = async (userData : any) => {
  const response = await axios.post(`${API_URL}/register`, userData);
  return response.data;
};

export const login = async (userData : any) => {
  const response = await axios.post(`${API_URL}/login`, userData);
  return response.data;
};