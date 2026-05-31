import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { useAuth } from '../context/AuthContext';
import { taskService } from '../services/taskService';
import Navbar from '../components/Navbar';
import toast from 'react-hot-toast';
import './Dashboard.css';

const Dashboard = () => {
  const { user } = useAuth();
  const [stats, setStats] = useState({ Pending: 0, InProgress: 0, Completed: 0 });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboardStats();
  }, []);

  const fetchDashboardStats = async () => {
    try {
      console.log('Fetching dashboard stats...');
      const data = await taskService.getDashboardStats();
      console.log('Dashboard stats received:', data);
      setStats(data);
    } catch (error) {
      console.error('Dashboard error:', error);
      toast.error('Failed to load dashboard stats: ' + (error.response?.data?.message || error.message));
    } finally {
      setLoading(false);
    }
  };

  const statCards = [
    {
      title: 'Pending',
      count: stats.Pending || 0,
      icon: '⏳',
      color: 'warning',
      gradient: 'linear-gradient(135deg, #f59e0b, #d97706)'
    },
    {
      title: 'In Progress',
      count: stats.InProgress || 0,
      icon: '🚀',
      color: 'info',
      gradient: 'linear-gradient(135deg, #3b82f6, #2563eb)'
    },
    {
      title: 'Completed',
      count: stats.Completed || 0,
      icon: '✅',
      color: 'success',
      gradient: 'linear-gradient(135deg, #10b981, #059669)'
    }
  ];

  const totalTasks = stats.Pending + stats.InProgress + stats.Completed;
  const completionRate = totalTasks > 0 ? ((stats.Completed / totalTasks) * 100).toFixed(1) : 0;

  return (
    <div className="page-wrapper">
      <Navbar />
      <div className="dashboard-container">
        <motion.div 
          className="dashboard-header"
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          <div>
            <h1 className="dashboard-title">
              Welcome back, <span className="highlight">{user?.username}</span>! 👋
            </h1>
            <p className="dashboard-subtitle">
              Here's what's happening with your tasks today
            </p>
          </div>
          <motion.div 
            className="completion-badge"
            whileHover={{ scale: 1.05 }}
          >
            <div className="completion-circle">
              <span className="completion-rate">{completionRate}%</span>
            </div>
            <span className="completion-label">Completion Rate</span>
          </motion.div>
        </motion.div>

        {loading ? (
          <div className="loading-state">
            <div className="spinner"></div>
            <p>Loading your dashboard...</p>
          </div>
        ) : (
          <>
            <div className="stats-grid">
              {statCards.map((stat, index) => (
                <motion.div
                  key={stat.title}
                  className={`stat-card stat-${stat.color}`}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: index * 0.1, duration: 0.5 }}
                  whileHover={{ y: -5, transition: { duration: 0.2 } }}
                >
                  <div className="stat-icon" style={{ background: stat.gradient }}>
                    {stat.icon}
                  </div>
                  <div className="stat-content">
                    <h3 className="stat-title">{stat.title}</h3>
                    <motion.p 
                      className="stat-count"
                      initial={{ scale: 0 }}
                      animate={{ scale: 1 }}
                      transition={{ delay: index * 0.1 + 0.3, type: 'spring', stiffness: 200 }}
                    >
                      {stat.count}
                    </motion.p>
                  </div>
                  <div className="stat-decoration"></div>
                </motion.div>
              ))}
            </div>

            <motion.div 
              className="dashboard-insights"
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.4, duration: 0.5 }}
            >
              <h2 className="insights-title">Quick Insights</h2>
              <div className="insights-grid">
                <div className="insight-card">
                  <span className="insight-icon">📊</span>
                  <div>
                    <p className="insight-label">Total Tasks</p>
                    <p className="insight-value">{totalTasks}</p>
                  </div>
                </div>
                <div className="insight-card">
                  <span className="insight-icon">⚡</span>
                  <div>
                    <p className="insight-label">Active Tasks</p>
                    <p className="insight-value">{stats.Pending + stats.InProgress}</p>
                  </div>
                </div>
                <div className="insight-card">
                  <span className="insight-icon">🎯</span>
                  <div>
                    <p className="insight-label">Success Rate</p>
                    <p className="insight-value">{completionRate}%</p>
                  </div>
                </div>
              </div>
            </motion.div>
          </>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
