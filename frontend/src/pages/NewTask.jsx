import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { motion } from 'framer-motion';
import { taskService } from '../services/taskService';
import { categoryService } from '../services/categoryService';
import Navbar from '../components/Navbar';
import Input from '../components/Input';
import Button from '../components/Button';
import toast from 'react-hot-toast';
import './NewTask.css';

const NewTask = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const editId = searchParams.get('edit');
  const isEditMode = !!editId;

  const [formData, setFormData] = useState({
    Title: '',
    Description: '',
    Status: 'Pending',
    Priority: 5,
    DueDate: '',
    Category: ''
  });
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingTask, setLoadingTask] = useState(isEditMode);

  useEffect(() => {
    fetchCategories();
    if (isEditMode) {
      fetchTask();
    }
  }, []);

  const fetchCategories = async () => {
    try {
      console.log('Fetching categories...');
      const data = await categoryService.getAllCategories();
      console.log('Categories received:', data);
      setCategories(data);
    } catch (error) {
      console.error('Categories error:', error);
      // Don't show error toast for categories as it's not critical
    }
  };

  const fetchTask = async () => {
    try {
      const data = await taskService.getTaskById(editId);
      console.log('Task data for edit:', data);
      setFormData({
        Title: data.Title || data.title || '',
        Description: data.Description || data.description || '',
        Status: data.Status || data.status || 'Pending',
        Priority: data.Priority || data.priority || 5,
        DueDate: (data.DueDate || data.dueDate).split('T')[0],
        Category: data.Category || data.category || ''
      });
    } catch (error) {
      toast.error('Failed to load task');
      navigate('/tasks');
    } finally {
      setLoadingTask(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'Priority' ? parseInt(value) : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (isEditMode) {
        await taskService.updateTask(editId, formData);
        toast.success('Task updated successfully! 🎉');
      } else {
        await taskService.createTask(formData);
        toast.success('Task created successfully! 🎉');
      }
      navigate('/tasks');
    } catch (error) {
      toast.error(error.response?.data?.message || `Failed to ${isEditMode ? 'update' : 'create'} task`);
    } finally {
      setLoading(false);
    }
  };

  if (loadingTask) {
    return (
      <div className="page-wrapper">
        <Navbar />
        <div className="loading-state">
          <div className="spinner"></div>
          <p>Loading task...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="page-wrapper">
      <Navbar />
      <div className="new-task-container">
        <motion.div
          className="new-task-card"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
        >
          <div className="form-header">
            <div>
              <h1 className="form-title">
                {isEditMode ? '✏️ Edit Task' : '➕ Create New Task'}
              </h1>
              <p className="form-subtitle">
                {isEditMode ? 'Update task details' : 'Fill in the details to create a new task'}
              </p>
            </div>
            <Button variant="secondary" onClick={() => navigate('/tasks')}>
              Cancel
            </Button>
          </div>

          <form onSubmit={handleSubmit} className="task-form">
            <Input
              label="Task Title"
              name="Title"
              value={formData.Title}
              onChange={handleChange}
              placeholder="Enter task title"
              required
              icon="📝"
            />

            <div className="input-group">
              <label className="input-label">
                Description <span className="required">*</span>
              </label>
              <textarea
                name="Description"
                value={formData.Description}
                onChange={handleChange}
                placeholder="Describe your task in detail..."
                required
                className="input-field"
                rows="5"
              />
            </div>

            <div className="form-row">
              <div className="input-group">
                <label className="input-label">
                  Status <span className="required">*</span>
                </label>
                <select
                  name="Status"
                  value={formData.Status}
                  onChange={handleChange}
                  required
                  className="input-field"
                >
                  <option value="Pending">Pending</option>
                  <option value="InProgress">In Progress</option>
                  <option value="Completed">Completed</option>
                </select>
              </div>

              <div className="input-group">
                <label className="input-label">
                  Priority (1-10) <span className="required">*</span>
                </label>
                <div className="priority-input-wrapper">
                  <input
                    type="range"
                    name="Priority"
                    value={formData.Priority}
                    onChange={handleChange}
                    min="1"
                    max="10"
                    className="priority-slider"
                  />
                  <span className="priority-value">{formData.Priority}</span>
                </div>
              </div>
            </div>

            <div className="form-row">
              <Input
                label="Due Date"
                type="date"
                name="DueDate"
                value={formData.DueDate}
                onChange={handleChange}
                required
                icon="📅"
              />

              <div className="input-group">
                <label className="input-label">Category</label>
                <select
                  name="Category"
                  value={formData.Category}
                  onChange={handleChange}
                  className="input-field"
                >
                  <option value="">Select a category</option>
                  {categories.map(cat => (
                    <option key={cat.Id || cat.id} value={cat.Name || cat.name}>
                      {cat.Name || cat.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div className="form-actions">
              <Button
                type="button"
                variant="secondary"
                onClick={() => navigate('/tasks')}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={loading} size="lg">
                {loading ? 'Saving...' : isEditMode ? 'Update Task' : 'Create Task'}
              </Button>
            </div>
          </form>
        </motion.div>
      </div>
    </div>
  );
};

export default NewTask;
