import { useState } from 'react';
import { taskService } from '../services/taskService';
import { categoryService } from '../services/categoryService';
import Navbar from '../components/Navbar';
import Button from '../components/Button';

const DebugPage = () => {
  const [tasks, setTasks] = useState(null);
  const [categories, setCategories] = useState(null);
  const [stats, setStats] = useState(null);

  const testTasks = async () => {
    try {
      const data = await taskService.getAllTasks();
      console.log('Tasks response:', data);
      setTasks(data);
    } catch (error) {
      console.error('Tasks error:', error);
      setTasks({ error: error.message });
    }
  };

  const testCategories = async () => {
    try {
      const data = await categoryService.getAllCategories();
      console.log('Categories response:', data);
      setCategories(data);
    } catch (error) {
      console.error('Categories error:', error);
      setCategories({ error: error.message });
    }
  };

  const testStats = async () => {
    try {
      const data = await taskService.getDashboardStats();
      console.log('Stats response:', data);
      setStats(data);
    } catch (error) {
      console.error('Stats error:', error);
      setStats({ error: error.message });
    }
  };

  return (
    <div className="page-wrapper">
      <Navbar />
      <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '2rem' }}>
        <h1 style={{ color: 'var(--accent-primary)', marginBottom: '2rem' }}>Debug API Responses</h1>
        
        <div style={{ display: 'flex', gap: '1rem', marginBottom: '2rem' }}>
          <Button onClick={testTasks}>Test Tasks API</Button>
          <Button onClick={testCategories}>Test Categories API</Button>
          <Button onClick={testStats}>Test Stats API</Button>
        </div>

        {tasks && (
          <div style={{ marginBottom: '2rem', background: 'var(--bg-card)', padding: '1rem', borderRadius: '8px' }}>
            <h2 style={{ color: 'var(--accent-primary)' }}>Tasks Response:</h2>
            <pre style={{ color: 'var(--text-primary)', overflow: 'auto' }}>
              {JSON.stringify(tasks, null, 2)}
            </pre>
          </div>
        )}

        {categories && (
          <div style={{ marginBottom: '2rem', background: 'var(--bg-card)', padding: '1rem', borderRadius: '8px' }}>
            <h2 style={{ color: 'var(--accent-primary)' }}>Categories Response:</h2>
            <pre style={{ color: 'var(--text-primary)', overflow: 'auto' }}>
              {JSON.stringify(categories, null, 2)}
            </pre>
          </div>
        )}

        {stats && (
          <div style={{ marginBottom: '2rem', background: 'var(--bg-card)', padding: '1rem', borderRadius: '8px' }}>
            <h2 style={{ color: 'var(--accent-primary)' }}>Stats Response:</h2>
            <pre style={{ color: 'var(--text-primary)', overflow: 'auto' }}>
              {JSON.stringify(stats, null, 2)}
            </pre>
          </div>
        )}
      </div>
    </div>
  );
};

export default DebugPage;
