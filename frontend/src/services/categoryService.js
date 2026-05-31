import api from './api';

export const categoryService = {
  getAllCategories: async () => {
    const response = await api.get('/category/list');
    return response.data;
  },

  getCategoryById: async (id) => {
    const response = await api.get(`/category/${id}`);
    return response.data;
  },

  createCategory: async (categoryData) => {
    const response = await api.post('/category/create', categoryData);
    return response.data;
  },

  updateCategory: async (id, categoryData) => {
    const response = await api.put(`/category/update/${id}`, categoryData);
    return response.data;
  },

  deleteCategory: async (id) => {
    const response = await api.delete(`/category/delete/${id}`);
    return response.data;
  },
};
