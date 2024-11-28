declare module '@features/auth/api' {
    export const login: (userData: any) => Promise<any>;
    export const register: (userData: any) => Promise<any>;
  }
  
  declare module '@shared/utils/auth' {
    export const setAuthToken: (token: string) => void;
    export const removeAuthToken: () => void;
    export const getAuthToken: () => string | null;
  }