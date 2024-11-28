export const setAuthToken = (token: string) => {
    document.cookie = `jwtToken=${token}; HttpOnly; Secure; SameSite=Strict`;
    localStorage.setItem('isAuthenticated', 'true');
  };
  
  export const removeAuthToken = () => {
    document.cookie = 'jwtToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
    localStorage.removeItem('isAuthenticated');
  };
  
  export const getAuthToken = (): string | null => {
    const cookies = document.cookie.split(';');
    for (const cookie of cookies) {
      const [name, value] = cookie.trim().split('=');
      if (name === 'jwtToken') {
        return value;
      }
    }
    return null;
  };