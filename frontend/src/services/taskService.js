import api from './api';

export const taskService = {
  getAllTasks: async () => {
    const response = await api.get('/task/all');
    return response.data;
  },

  getTaskById: async (id) => {
    const response = await api.get(`/task/${id}`);
    return response.data;
  },

  createTask: async (taskData) => {
    const response = await api.post('/task/create', taskData);
    return response.data;
  },

  updateTask: async (id, taskData) => {
    const response = await api.put(`/task/update/${id}`, taskData);
    return response.data;
  },

  deleteTask: async (id) => {
    const response = await api.delete(`/task/delete/${id}`);
    return response.data;
  },

  getDashboardStats: async () => {
    const response = await api.get('/task/dashboard');
    return response.data;
  },

  reassignTask: async (id, newUserId) => {
    const response = await api.put(`/task/reassign/${id}`, { NewUserId: newUserId });
    return response.data;
  },
};
