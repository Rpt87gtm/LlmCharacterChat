import UserChatsPage from "@/pages/chat/UserChatsPage.vue";
import ChatPage from "@/pages/chat/ChatPage.vue";

export const ChatRoutes = [
  {
    path: "/chats",
    name: "ChatsList",
    component: UserChatsPage,
    meta: { requiresAuth: true },
  },
  {
    path: "/chats/:chatId",
    name: "Chat",
    component: ChatPage,
    meta: { requiresAuth: true },
  }
];
