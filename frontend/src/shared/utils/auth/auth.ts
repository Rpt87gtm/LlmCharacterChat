export const setAuthToken = (token: string) => {
    localStorage.setItem('jwtToken', token)
    localStorage.setItem('isAuthenticated', 'true');
  };
  
  export const removeAuthToken = () => {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('isAuthenticated');
  };
  
  export const getAuthToken = (): string | null => {
    return localStorage.getItem('jwtToken');
  };