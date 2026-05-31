import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/authService';
import Navbar from '../components/Navbar';
import Button from '../components/Button';
import toast from 'react-hot-toast';
import './UserProfile.css';

const UserProfile = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      const data = await authService.getProfile();
      setProfile(data);
    } catch (error) {
      console.error('Failed to load profile');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    logout();
    toast.success('Logged out successfully');
    navigate('/login');
  };

  if (loading) {
    return (
      <div className="page-wrapper">
        <Navbar />
        <div className="loading-state">
          <div className="spinner"></div>
          <p>Loading profile...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="page-wrapper">
      <Navbar />
      <div className="profile-container">
        <motion.div
          className="profile-card"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
        >
          <div className="profile-header">
            <div className="profile-avatar">
              <span className="avatar-text">
                {user?.username?.charAt(0).toUpperCase()}
              </span>
            </div>
            <div className="profile-info">
              <h1 className="profile-name">{user?.username}</h1>
              <span className={`role-badge role-${user?.role?.toLowerCase()}`}>
                {user?.role}
              </span>
            </div>
          </div>

          <div className="profile-details">
            <h2 className="section-title">Account Information</h2>
            
            <div className="detail-grid">
              <div className="detail-card">
                <span className="detail-icon">👤</span>
                <div>
                  <p className="detail-label">Username</p>
                  <p className="detail-text">{profile?.username || user?.username}</p>
                </div>
              </div>

              <div className="detail-card">
                <span className="detail-icon">🎭</span>
                <div>
                  <p className="detail-label">Role</p>
                  <p className="detail-text">{profile?.role || user?.role}</p>
                </div>
              </div>

              <div className="detail-card">
                <span className="detail-icon">🆔</span>
                <div>
                  <p className="detail-label">User ID</p>
                  <p className="detail-text">#{profile?.userId}</p>
                </div>
              </div>

              <div className="detail-card">
                <span className="detail-icon">✅</span>
                <div>
                  <p className="detail-label">Account Status</p>
                  <p className="detail-text status-active">Active</p>
                </div>
              </div>
            </div>
          </div>

          <div className="profile-stats">
            <h2 className="section-title">Quick Stats</h2>
            <div className="stats-row">
              <div className="stat-item">
                <span className="stat-icon">📊</span>
                <div>
                  <p className="stat-value">View Dashboard</p>
                  <p className="stat-label">See your task statistics</p>
                </div>
              </div>
              <div className="stat-item">
                <span className="stat-icon">📝</span>
                <div>
                  <p className="stat-value">Manage Tasks</p>
                  <p className="stat-label">Create and organize tasks</p>
                </div>
              </div>
            </div>
          </div>

          <div className="profile-actions">
            <Button
              variant="secondary"
              onClick={() => navigate('/dashboard')}
              fullWidth
            >
              Go to Dashboard
            </Button>
            <Button
              variant="danger"
              onClick={handleLogout}
              fullWidth
            >
              Logout
            </Button>
          </div>
        </motion.div>

        <motion.div
          className="profile-sidebar"
          initial={{ opacity: 0, x: 20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ delay: 0.2 }}
        >
          <div className="sidebar-card">
            <h3 className="sidebar-title">Quick Actions</h3>
            <div className="quick-actions">
              <button
                className="quick-action-btn"
                onClick={() => navigate('/tasks/new')}
              >
                <span className="action-icon">➕</span>
                <span>Create New Task</span>
              </button>
              <button
                className="quick-action-btn"
                onClick={() => navigate('/tasks')}
              >
                <span className="action-icon">📋</span>
                <span>View All Tasks</span>
              </button>
              <button
                className="quick-action-btn"
                onClick={() => navigate('/dashboard')}
              >
                <span className="action-icon">📊</span>
                <span>View Dashboard</span>
              </button>
            </div>
          </div>

          <div className="sidebar-card">
            <h3 className="sidebar-title">Account Tips</h3>
            <ul className="tips-list">
              <li>💡 Use priorities to organize your tasks</li>
              <li>🎯 Set realistic due dates</li>
              <li>📂 Categorize tasks for better organization</li>
              <li>✅ Update task status regularly</li>
            </ul>
          </div>
        </motion.div>
      </div>
    </div>
  );
};

export default UserProfile;
