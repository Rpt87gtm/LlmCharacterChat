import { createRouter, createWebHistory } from 'vue-router';
import { BaseRoutes } from '@/app/routers/BaseRoutes';

const routes = [...BaseRoutes]



export const router = createRouter({
  history: createWebHistory(),
  routes,
});
