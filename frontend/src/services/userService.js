import api from './api';

export const userService = {
  getAllUsers: async () => {
    const response = await api.get('/auth/users');
    return response.data;
  },
};
