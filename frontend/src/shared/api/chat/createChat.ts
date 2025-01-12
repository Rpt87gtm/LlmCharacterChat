import apiClient from '@/shared/api';  
import { SERVER_API_URL } from '../apiClient';
import { ChatMessage } from '@/shared/api/chat/chatApi';

const CHAT_API_URL = `${SERVER_API_URL}/chats`;

export interface ChatDetails {
  chatId: string;
  characterName: string;
  messages: ChatMessage[];
}

export interface ChatCreateRequest {
  characterId: string;
}

export const createChat = async (data: ChatCreateRequest): Promise<ChatDetails> => {
  const response = await apiClient.post(CHAT_API_URL, data);
  return response.data;
};
