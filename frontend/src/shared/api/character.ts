import apiClient from '@/shared/api';  

const API_URL = "http://localhost:5190/api/characters";

export interface Character {
  Id: string;
  Name: string;
  SystemPrompt: string;
}

interface CharacterQuery {
  Name?: string;
  SortBy?: string;
  IsDescending?: boolean;
}

interface QueryPage {
  PageNumber: number;
  PageSize: number;
}
interface PaginatedResponse {
  data: Character[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
export const fetchCharacters = async (
  characterQuery: CharacterQuery = {},
  queryPage: QueryPage = { PageNumber: 1, PageSize: 10 }
): Promise<PaginatedResponse> => {
  const response = await apiClient.get(`${API_URL}/GetAllCharacters`, {
    params: {
      ...characterQuery,
      ...queryPage,
    },
  });
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
export const updateCharacter = async (id: string, characterData: any) => {
  const response = await apiClient.put(`${API_URL}/${id}`, characterData);
  return response.data;
};
