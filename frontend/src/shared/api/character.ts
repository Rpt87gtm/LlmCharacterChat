import apiClient from '@/shared/api';  

const API_URL = "http://localhost:5190/api/characters";

export const fetchCharacters = async (params = {}) => {
  const response = await apiClient.get(`${API_URL}/GetAllCharacters`, { params });
  return response.data;
};

export const fetchCharacterById = async (id: string) => {
  const response = await apiClient.get(`${API_URL}/${id}`);
  return response.data;
};

export const createCharacter = async (characterData: any) => {
  const response = await apiClient.post(API_URL, characterData);
  return response.data;
};

export const deleteCharacter = async (id: string) => {
  await apiClient.delete(`${API_URL}/${id}`);
};
