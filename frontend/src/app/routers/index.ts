import { createRouter, createWebHistory } from 'vue-router';
import { BaseRoutes } from '@/app/routers/BaseRoutes';
import { AccountRoutes } from '@/app/routers/AccountRoutes';
import { CharacterRoutes } from './CharacterRoutes';

const routes = [...BaseRoutes, ...AccountRoutes, ...CharacterRoutes];



export const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem('isAuthenticated');
  if (to.matched.some(record => record.meta.requiresAuth) && !isAuthenticated) {
    next('/login');
  } else {
    next();
  }
});