import { HomePage } from "@/pages/home/index";
import { Example } from "@/pages/examplePage";

export const BaseRoutes = [
  {
    path: '/',
    name: 'home',
    component: HomePage,
    meta: {requiresAuth: true},

  },
  {
    path: '/example',
    name: 'ExamplePage',
    component: Example,
  },
];
