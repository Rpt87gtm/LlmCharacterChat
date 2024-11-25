import { createRouter, createWebHistory } from 'vue-router';
import { routes } from '@/shared/config/routes';

export const router = createRouter({
  history: createWebHistory(),
  routes,
});
