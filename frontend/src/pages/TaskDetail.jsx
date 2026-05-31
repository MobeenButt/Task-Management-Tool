import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { taskService } from '../services/taskService';
import { userService } from '../services/userService';
import { useAuth } from '../context/AuthContext';
import Navbar from '../components/Navbar';
import Button from '../components/Button';
import toast from 'react-hot-toast';
import './TaskDetail.css';

const TaskDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const [task, setTask] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showReassignModal, setShowReassignModal] = useState(false);
  const [users, setUsers] = useState([]);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [reassigning, setReassigning] = useState(false);

  useEffect(() => {
    fetchTask();
    if (isAdmin) {
      fetchUsers();
    }
  }, [id]);

  const fetchTask = async () => {
    try {
      const data = await taskService.getTaskById(id);
      console.log('Task detail received:', data);
      setTask(data);
      setSelectedUserId(data.UserId || data.userId);
    } catch (error) {
      toast.error('Failed to load task details');
      navigate('/tasks');
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

  const handleReassign = async () => {
    if (!selectedUserId) {
      toast.error('Please select a user');
      return;
    }

    setReassigning(true);
    try {
      await taskService.reassignTask(id, parseInt(selectedUserId));
      toast.success('Task reassigned successfully! 🎉');
      setShowReassignModal(false);
      fetchTask(); // Refresh task data
    } catch (error) {
      toast.error('Failed to reassign task');
    } finally {
      setReassigning(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this task?')) return;

    try {
      await taskService.deleteTask(id);
      toast.success('Task deleted successfully');
      navigate('/tasks');
    } catch (error) {
      toast.error('Failed to delete task');
    }
  };

  const getPriorityLabel = (priority) => {
    if (priority >= 8) return { label: 'High', color: 'high' };
    if (priority >= 5) return { label: 'Medium', color: 'medium' };
    return { label: 'Low', color: 'low' };
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'Completed': return 'success';
      case 'InProgress': return 'info';
      case 'Pending': return 'warning';
      default: return 'default';
    }
  };

  if (loading) {
    return (
      <div className="page-wrapper">
        <Navbar />
        <div className="loading-state">
          <div className="spinner"></div>
          <p>Loading task details...</p>
        </div>
      </div>
    );
  }

  if (!task) return null;

  const taskId = task.Id || task.id;
  const taskTitle = task.Title || task.title;
  const taskDescription = task.Description || task.description;
  const taskStatus = task.Status || task.status;
  const taskPriority = task.Priority || task.priority;
  const taskDueDate = task.DueDate || task.dueDate;
  const taskCategory = task.Category || task.category;

  const priority = getPriorityLabel(taskPriority);

  return (
    <div className="page-wrapper">
      <Navbar />
      <div className="task-detail-container">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="task-detail-card"
        >
          <div className="detail-header">
            <Button variant="secondary" onClick={() => navigate('/tasks')}>
              ← Back to Tasks
            </Button>
            <div className="header-actions">
              {isAdmin && (
                <Button onClick={() => setShowReassignModal(true)}>
                  Reassign Task
                </Button>
              )}
              <Button onClick={() => navigate(`/tasks/new?edit=${id}`)}>
                Edit Task
              </Button>
              <Button variant="danger" onClick={handleDelete}>
                Delete Task
              </Button>
            </div>
          </div>

          <div className="detail-badges">
            <span className={`status-badge status-${getStatusColor(taskStatus)}`}>
              {taskStatus === 'InProgress' ? 'In Progress' : taskStatus}
            </span>
            <span className={`priority-badge priority-${priority.color}`}>
              {priority.label} Priority ({taskPriority}/10)
            </span>
          </div>

          <h1 className="detail-title">{taskTitle}</h1>

          <div className="detail-section">
            <h3 className="section-title">Description</h3>
            <p className="detail-description">{taskDescription || 'No description provided'}</p>
          </div>

          <div className="detail-grid">
            <div className="detail-item">
              <span className="detail-label">📅 Due Date</span>
              <span className="detail-value">
                {new Date(taskDueDate).toLocaleDateString('en-US', {
                  weekday: 'long',
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric'
                })}
              </span>
            </div>

            {taskCategory && (
              <div className="detail-item">
                <span className="detail-label">🏷️ Category</span>
                <span className="detail-value">{taskCategory}</span>
              </div>
            )}

            <div className="detail-item">
              <span className="detail-label">👤 Assigned To</span>
              <span className="detail-value">{task.Username || task.username || 'Unknown'}</span>
            </div>

            <div className="detail-item">
              <span className="detail-label">📊 Status</span>
              <span className="detail-value">
                {taskStatus === 'InProgress' ? 'In Progress' : taskStatus}
              </span>
            </div>

            <div className="detail-item">
              <span className="detail-label">⚡ Priority Level</span>
              <span className="detail-value">{taskPriority} / 10</span>
            </div>
          </div>

          <div className="detail-timeline">
            <h3 className="section-title">Task Timeline</h3>
            <div className="timeline">
              <div className="timeline-item">
                <div className="timeline-dot"></div>
                <div className="timeline-content">
                  <p className="timeline-label">Created</p>
                  <p className="timeline-date">Task was created</p>
                </div>
              </div>
              <div className={`timeline-item ${taskStatus !== 'Pending' ? 'active' : ''}`}>
                <div className="timeline-dot"></div>
                <div className="timeline-content">
                  <p className="timeline-label">In Progress</p>
                  <p className="timeline-date">
                    {taskStatus !== 'Pending' ? 'Currently working on it' : 'Not started yet'}
                  </p>
                </div>
              </div>
              <div className={`timeline-item ${taskStatus === 'Completed' ? 'active' : ''}`}>
                <div className="timeline-dot"></div>
                <div className="timeline-content">
                  <p className="timeline-label">Completed</p>
                  <p className="timeline-date">
                    {taskStatus === 'Completed' ? 'Task completed!' : 'Pending completion'}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </motion.div>

        {/* Reassign Modal */}
        {showReassignModal && (
          <div className="modal-overlay" onClick={() => setShowReassignModal(false)}>
            <motion.div 
              className="modal-content"
              initial={{ opacity: 0, scale: 0.9 }}
              animate={{ opacity: 1, scale: 1 }}
              onClick={(e) => e.stopPropagation()}
            >
              <h3 className="modal-title">Reassign Task</h3>
              <p className="modal-subtitle">Select a user to assign this task to:</p>
              
              <div className="modal-form">
                <select
                  value={selectedUserId}
                  onChange={(e) => setSelectedUserId(e.target.value)}
                  className="modal-select"
                >
                  <option value="">Select User</option>
                  {users.map(user => (
                    <option key={user.Id || user.id} value={user.Id || user.id}>
                      {user.Username || user.username} ({user.Role || user.role})
                    </option>
                  ))}
                </select>
              </div>

              <div className="modal-actions">
                <Button 
                  variant="secondary" 
                  onClick={() => setShowReassignModal(false)}
                >
                  Cancel
                </Button>
                <Button 
                  onClick={handleReassign}
                  disabled={reassigning || !selectedUserId}
                >
                  {reassigning ? 'Reassigning...' : 'Reassign Task'}
                </Button>
              </div>
            </motion.div>
          </div>
        )}
      </div>
    </div>
  );
};

export default TaskDetail;
