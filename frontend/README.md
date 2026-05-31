# TaskFlow - Task Management Frontend

A modern, beautiful React frontend for the Task Management System with a unique cyberpunk-inspired aesthetic.

## 🎨 Design Features

- **Unique Typography**: Syne (display) + JetBrains Mono (monospace)
- **Cyberpunk Theme**: Dark background with warm orange/amber accents
- **Smooth Animations**: Framer Motion for delightful micro-interactions
- **Responsive Design**: Works seamlessly on all devices
- **Animated Background**: Dynamic grid pattern with glowing effects

## 🚀 Getting Started

### Prerequisites

- Node.js (v16 or higher)
- npm or yarn
- Backend API running on `https://localhost:7123`

### Installation

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Configure the API URL (if different):
Edit `.env` file:
```
VITE_API_URL=https://localhost:7123
```

4. Start the development server:
```bash
npm run dev
```

5. Open your browser and navigate to:
```
http://localhost:3000
```

## 📦 Project Structure

```
frontend/
├── src/
│   ├── components/          # Reusable UI components
│   │   ├── Button.jsx
│   │   ├── Input.jsx
│   │   └── Navbar.jsx
│   ├── context/            # React Context
│   │   └── AuthContext.jsx
│   ├── pages/              # Page components
│   │   ├── Login.jsx
│   │   ├── Signup.jsx
│   │   ├── Dashboard.jsx
│   │   ├── TaskList.jsx
│   │   ├── TaskDetail.jsx
│   │   ├── NewTask.jsx
│   │   └── UserProfile.jsx
│   ├── services/           # API services
│   │   ├── api.js
│   │   ├── authService.js
│   │   ├── taskService.js
│   │   └── categoryService.js
│   ├── App.jsx             # Main app component
│   ├── main.jsx            # Entry point
│   └── index.css           # Global styles
├── index.html
├── package.json
└── vite.config.js
```

## 🎯 Features

### Authentication
- ✅ User registration
- ✅ User login
- ✅ JWT token management
- ✅ Protected routes
- ✅ Auto-redirect on token expiry

### Dashboard
- ✅ Task statistics (Pending, In Progress, Completed)
- ✅ Completion rate visualization
- ✅ Quick insights
- ✅ Animated stat cards

### Task Management
- ✅ View all tasks with filters
- ✅ Search tasks
- ✅ Filter by status
- ✅ Create new tasks
- ✅ Edit existing tasks
- ✅ Delete tasks
- ✅ View task details
- ✅ Priority levels (1-10)
- ✅ Task categories
- ✅ Due dates

### User Profile
- ✅ View account information
- ✅ Quick actions
- ✅ Logout functionality

## 🎨 Color Palette

```css
Primary Background: #0a0e1a
Secondary Background: #111827
Card Background: #151d2e

Accent Primary: #ff6b35 (Orange)
Accent Secondary: #f7931e (Amber)
Accent Tertiary: #ffd23f (Yellow)

Text Primary: #e8eaed
Text Secondary: #9ca3af
Text Muted: #6b7280
```

## 🔧 Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build

## 🌐 API Integration

The frontend connects to the ASP.NET Core backend API:

- **Base URL**: `https://localhost:7123/api`
- **Authentication**: JWT Bearer tokens
- **CORS**: Configured for `http://localhost:3000`

### API Endpoints Used

- `POST /auth/register` - User registration
- `POST /auth/login` - User login
- `GET /auth/profile` - Get user profile
- `GET /task/all` - Get all tasks
- `GET /task/{id}` - Get task by ID
- `POST /task/create` - Create task
- `PUT /task/update/{id}` - Update task
- `DELETE /task/delete/{id}` - Delete task
- `GET /task/dashboard` - Get dashboard stats
- `GET /category/list` - Get categories

## 🔐 Demo Credentials

```
Username: admin
Password: Admin@123
```

## 🎭 Technologies Used

- **React 18** - UI library
- **Vite** - Build tool
- **React Router** - Navigation
- **Framer Motion** - Animations
- **Axios** - HTTP client
- **React Hot Toast** - Notifications

## 📱 Responsive Breakpoints

- Mobile: < 768px
- Tablet: 768px - 1024px
- Desktop: > 1024px

## 🚨 Troubleshooting

### CORS Issues
If you encounter CORS errors, ensure the backend has CORS configured for `http://localhost:3000`.

### SSL Certificate Errors
The backend uses HTTPS with a self-signed certificate. You may need to accept the certificate in your browser.

### API Connection Issues
1. Verify the backend is running
2. Check the API URL in `.env`
3. Ensure the backend CORS policy includes your frontend URL

## 📄 License

This project is part of the Task Management System.
