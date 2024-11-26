import { HomePage } from "@/pages/home/index";
import { Example } from "@/pages/examplePage";

export const routes = [
  {
    path: '/',
    name: 'home',
    component: HomePage,
  },
  {
    path: '/example',
    name: 'ExamplePage',
    component: Example,
  },
];
