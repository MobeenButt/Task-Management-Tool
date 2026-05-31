import { createContext, useContext, useState, useEffect } from 'react';
import { authService } from '../services/authService';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is logged in on mount
    if (authService.isAuthenticated()) {
      const userData = authService.getUser();
      setUser(userData);
      console.log('User loaded from storage:', userData);
    }
    setLoading(false);
  }, []);

  const login = async (username, password) => {
    const data = await authService.login(username, password);
    const userData = { username: data.username, role: data.role };
    setUser(userData);
    console.log('User logged in:', userData);
    // Small delay to ensure localStorage is updated
    await new Promise(resolve => setTimeout(resolve, 100));
    return data;
  };

  const register = async (username, password) => {
    const data = await authService.register(username, password);
    const userData = { username: data.username, role: data.role };
    setUser(userData);
    console.log('User registered:', userData);
    // Small delay to ensure localStorage is updated
    await new Promise(resolve => setTimeout(resolve, 100));
    return data;
  };

  const logout = () => {
    authService.logout();
    setUser(null);
    console.log('User logged out');
  };

  const value = {
    user,
    login,
    register,
    logout,
    isAuthenticated: !!user,
    isAdmin: user?.role === 'Admin',
  };

  if (loading) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        background: 'var(--bg-primary)',
        color: 'var(--accent-primary)',
        fontFamily: 'var(--font-mono)',
        fontSize: '1.2rem'
      }}>
        <div className="animate-fade-in">Loading...</div>
      </div>
    );
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
