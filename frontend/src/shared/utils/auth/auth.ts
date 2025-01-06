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

export const getUserIdFromToken = (): string | null => {
  const token = getAuthToken();
  if (!token) return null;

  try {
    const payload = JSON.parse(atob(token.split('.')[1])); 
    return payload.userId || null; 
  } catch (error) {
    console.error("Invalid token format:", error);
    return null;
  }
};
  