import apiClient from '@/shared/api';  
import { SERVER_API_URL } from '../apiClient';
import { ChatCreateRequest, ChatDetails } from './chatApi';

const CHAT_API_URL = `${SERVER_API_URL}/chats`;

export const createChat = async (data: ChatCreateRequest): Promise<ChatDetails> => {
  const response = await apiClient.post(CHAT_API_URL, data);
  return response.data;
};
