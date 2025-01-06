import apiClient from '@/shared/api';  
import { SERVER_API_URL } from '../apiClient';
import { AxiosResponse } from "axios";

const CHAT_API_URL = `${SERVER_API_URL}/chats`;

export interface ChatMessage {
    id: number;
    role: string; 
    content: string;
    chatHistoryId: string;
    sentAt: string;
  }
  
  export interface ChatDetails {
    chatId: string;
    characterName: string;
    messages: ChatMessage[];
  }
  
  export interface ChatCreateRequest {
    characterId: string;
  }
  
  export interface ChatMessageCreateRequest {
    chatId: string;
    role: string;
    content: string;
  }
  
  export interface ChatMessageResponse {
      userMessage: ChatMessage;
      assistantMessage: ChatMessage;
    }
export const fetchChatDetails = async (chatId: string): Promise<ChatDetails> => {
    const response = await apiClient.get(`${CHAT_API_URL}/${chatId}`);
    return response.data;
};

  
  export const sendMessage = async (data: ChatMessageCreateRequest): Promise<ChatMessageResponse> => {
    const response = await apiClient.post(`${CHAT_API_URL}/messages`, data);
    return response.data;
  };
  
