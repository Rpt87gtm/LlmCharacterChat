import { reactive } from 'vue';

const SOCKET_URL = "ws://localhost:5190/ws";

export interface ChatMessage {
    Id?: number;
    Role: string;
    Content: string;
    ChatHistoryId: string;
    SentAt?: string;
}

export interface ChatResponseDto {
    ChatId: string;
    CharacterName: string;
    Messages: ChatMessage[];
}

export interface ChatMessageResponse {
    UserMessage?: ChatMessage;
    AssistantMessage?: ChatMessage;
    error?: string;
}

export const chatSocketState = reactive({
    socket: null as WebSocket | null,
    messages: [] as ChatMessage[],
    isConnected: false,
    error: null as string | null,
    characterName: "",
});



let shouldReconnect = true;

export const connectWebSocket = (chatId: string, 
    onMessageCallback: (message: ChatMessageResponse,) => void, 
    onLoading:() => void): void => {
    const socket = new WebSocket(SOCKET_URL);

    socket.onopen = () => {
        console.log("WebSocket connected");
        chatSocketState.isConnected = true;
        chatSocketState.socket = socket;
        chatSocketState.error = null;

        socket.send(JSON.stringify({ ChatId: chatId }));
    };

    socket.onmessage = (event) => {
      try {
          const data = JSON.parse(event.data) as ChatResponseDto | ChatMessageResponse;
  
          if ("error" in data) {
              console.error("Server error:", data.error);
              chatSocketState.error = data.error ?? null;
              return;
          }
  
          if ("CharacterName" in data) {
              chatSocketState.characterName = data.CharacterName;
              chatSocketState.messages = data.Messages;
              onLoading();
              return;
          }
  
          if (data.UserMessage) {
              chatSocketState.messages.push(data.UserMessage);
              onMessageCallback(data); 
          }
          if (data.AssistantMessage) {
              chatSocketState.messages.push(data.AssistantMessage);
              onMessageCallback(data); 
          }
      } catch (error) {
          console.error("Failed to parse WebSocket message:", error);
          chatSocketState.error = "Failed to parse server response.";
      }
  };

    socket.onclose = () => {
        console.log("WebSocket disconnected");
        chatSocketState.isConnected = false;
        chatSocketState.socket = null;
        chatSocketState.error = "Connection closed. Reconnecting...";

        if (shouldReconnect) {
            setTimeout(() => {
                connectWebSocket(chatId, onMessageCallback, onLoading);
            }, 5000);
        }
    };

    socket.onerror = (error) => {
        console.error("WebSocket error:", error);
        chatSocketState.error = "WebSocket error. Please try again.";
    };
};

export const sendSocketMessage = (message: ChatMessage): void => {
    if (chatSocketState.socket && chatSocketState.isConnected) {
        try {
            chatSocketState.socket.send(JSON.stringify(message));
        } catch (error) {
            console.error("Failed to send message:", error);
            chatSocketState.error = "Failed to send message.";
        }
    } else {
        console.error("WebSocket is not connected");
        chatSocketState.error = "WebSocket is not connected.";
    }
};

export const disconnectWebSocket = (): void => {
    shouldReconnect = false; 
    if (chatSocketState.socket) {
        chatSocketState.socket.close();
        chatSocketState.socket = null;
        chatSocketState.isConnected = false;
        chatSocketState.messages = [];
        chatSocketState.characterName = "";
        chatSocketState.error = null;
    }
};