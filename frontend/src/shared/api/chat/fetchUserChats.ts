import apiClient from '@/shared/api';  
import { SERVER_API_URL } from '../apiClient';

const CHAT_API_URL = `${SERVER_API_URL}/chats`;
export interface ChatSummary {
  chatId: string;
  characterName: string;
  lastMessageContent?: string;
  updatedAt: string; 
}
export const fetchUserChats = async (): Promise<ChatSummary[]> => {
  const response = await apiClient.get(`${CHAT_API_URL}`);
  return response.data;
};
