import api from './api';

export const authService = {
  register: async (username, password) => {
    const response = await api.post('/auth/register', { username, password });
    const token = response.data.Token || response.data.token;
    if (token) {
      localStorage.setItem('token', token);
      localStorage.setItem('username', response.data.username);
      localStorage.setItem('role', response.data.role);
      // Update axios default header immediately
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    }
    return response.data;
  },

  login: async (username, password) => {
    const response = await api.post('/auth/login', { username, password });
    const token = response.data.Token || response.data.token;
    if (token) {
      localStorage.setItem('token', token);
      localStorage.setItem('username', response.data.username);
      localStorage.setItem('role', response.data.role);
      // Update axios default header immediately
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      console.log('Token stored:', token.substring(0, 20) + '...');
    }
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    delete api.defaults.headers.common['Authorization'];
  },

  getProfile: async () => {
    const response = await api.get('/auth/profile');
    return response.data;
  },

  isAuthenticated: () => {
    return !!localStorage.getItem('token');
  },

  getUser: () => {
    return {
      username: localStorage.getItem('username'),
      role: localStorage.getItem('role'),
    };
  },
};
