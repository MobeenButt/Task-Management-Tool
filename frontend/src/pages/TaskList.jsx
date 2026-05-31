import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { taskService } from '../services/taskService';
import { userService } from '../services/userService';
import { useAuth } from '../context/AuthContext';
import Navbar from '../components/Navbar';
import Button from '../components/Button';
import toast from 'react-hot-toast';
import './TaskList.css';

const TaskList = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('All');
  const [userFilter, setUserFilter] = useState('All');
  const [searchTerm, setSearchTerm] = useState('');
  const [users, setUsers] = useState([]);
  const { isAdmin } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    fetchTasks();
    if (isAdmin) {
      fetchUsers();
    }
  }, []);

  const fetchTasks = async () => {
    try {
      console.log('Fetching tasks...');
      const data = await taskService.getAllTasks();
      console.log('Tasks received:', data);
      setTasks(data);
    } catch (error) {
      console.error('Tasks error:', error);
      toast.error('Failed to load tasks: ' + (error.response?.data?.message || error.message));
    } finally {
      setLoading(false);
    }
  };

  const fetchUsers = async () => {
    try {
      const data = await userService.getAllUsers();
      setUsers(data);
    } catch (error) {
      console.error('Failed to load users');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this task?')) return;

    try {
      await taskService.deleteTask(id);
      toast.success('Task deleted successfully');
      fetchTasks();
    } catch (error) {
      toast.error('Failed to delete task');
    }
  };

  const filteredTasks = tasks.filter(task => {
    const taskStatus = task.Status || task.status;
    const taskTitle = task.Title || task.title || '';
    const taskDescription = task.Description || task.description || '';
    const taskUserId = task.UserId || task.userId;
    
    const matchesFilter = filter === 'All' || taskStatus === filter;
    const matchesUserFilter = userFilter === 'All' || taskUserId === parseInt(userFilter);
    const matchesSearch = taskTitle.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         taskDescription.toLowerCase().includes(searchTerm.toLowerCase());
    return matchesFilter && matchesUserFilter && matchesSearch;
  });

  const getPriorityColor = (priority) => {
    if (priority >= 8) return 'high';
    if (priority >= 5) return 'medium';
    return 'low';
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'Completed': return 'success';
      case 'InProgress': return 'info';
      case 'Pending': return 'warning';
      default: return 'default';
    }
  };

  return (
    <div className="page-wrapper">
      <Navbar />
      <div className="task-list-container">
        <motion.div 
          className="task-list-header"
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
        >
          <div>
            <h1 className="page-title">Task Management</h1>
            <p className="page-subtitle">Organize and track your work</p>
          </div>
          <Button onClick={() => navigate('/tasks/new')}>
            + New Task
          </Button>
        </motion.div>

        <motion.div 
          className="task-filters"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.2 }}
        >
          <div className="search-box">
            <span className="search-icon">🔍</span>
            <input
              type="text"
              placeholder="Search tasks..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="search-input"
            />
          </div>

          <div className="filter-buttons">
            {['All', 'Pending', 'InProgress', 'Completed'].map((status) => (
              <button
                key={status}
                onClick={() => setFilter(status)}
                className={`filter-btn ${filter === status ? 'active' : ''}`}
              >
                {status === 'InProgress' ? 'In Progress' : status}
              </button>
            ))}
          </div>

          {isAdmin && (
            <div className="user-filter">
              <select
                value={userFilter}
                onChange={(e) => setUserFilter(e.target.value)}
                className="user-filter-select"
              >
                <option value="All">All Users</option>
                {users.map(user => (
                  <option key={user.Id || user.id} value={user.Id || user.id}>
                    {user.Username || user.username} ({user.Role || user.role})
                  </option>
                ))}
              </select>
            </div>
          )}
        </motion.div>

        {loading ? (
          <div className="loading-state">
            <div className="spinner"></div>
            <p>Loading tasks...</p>
          </div>
        ) : filteredTasks.length === 0 ? (
          <motion.div 
            className="empty-state"
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
          >
            <div className="empty-icon">📋</div>
            <h3>No tasks found</h3>
            <p>Create your first task to get started</p>
            <Button onClick={() => navigate('/tasks/new')}>
              Create Task
            </Button>
          </motion.div>
        ) : (
          <div className="tasks-grid">
            {filteredTasks.map((task, index) => (
              <motion.div
                key={task.Id || task.id}
                className="task-card"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: index * 0.05 }}
                whileHover={{ y: -5 }}
              >
                <div className="task-card-header">
                  <span className={`status-badge status-${getStatusColor(task.Status || task.status)}`}>
                    {(task.Status || task.status) === 'InProgress' ? 'In Progress' : (task.Status || task.status)}
                  </span>
                  <span className={`priority-badge priority-${getPriorityColor(task.Priority || task.priority)}`}>
                    Priority: {task.Priority || task.priority}
                  </span>
                </div>

                <h3 className="task-title">{task.Title || task.title}</h3>
                <p className="task-description">{task.Description || task.description}</p>

                <div className="task-meta">
                  {(task.Category || task.category) && (
                    <span className="task-category">
                      🏷️ {task.Category || task.category}
                    </span>
                  )}
                  <span className="task-due-date">
                    📅 {new Date(task.DueDate || task.dueDate).toLocaleDateString()}
                  </span>
                  {(task.Username || task.username) && (
                    <span className="task-assignee">
                      👤 {task.Username || task.username}
                    </span>
                  )}
                </div>

                <div className="task-actions">
                  <Link to={`/tasks/${task.Id || task.id}`} className="action-btn view-btn">
                    View Details
                  </Link>
                  <button
                    onClick={() => handleDelete(task.Id || task.id)}
                    className="action-btn delete-btn"
                  >
                    Delete
                  </button>
                </div>
              </motion.div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default TaskList;
