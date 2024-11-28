import { LoginPage, RegisterPage} from "@/pages/auth/ui";


export const AccountRoutes = [
    {
      path: '/login',
      name: 'Login',
      component: LoginPage,
    },
    {
      path: '/register',
      name: 'Register',
      component: RegisterPage,
    },
  ];