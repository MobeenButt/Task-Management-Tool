#!/bin/bash

# Task Management System - Automated Setup Script for Mac/Linux
# This script helps you set up and run the Task Management System quickly

echo "🚀 Task Management System - Quick Setup"
echo "======================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to check .NET version
check_dotnet_version() {
    if command_exists dotnet; then
        version=$(dotnet --version 2>/dev/null)
        if [ $? -eq 0 ]; then
            major_version=$(echo $version | cut -d. -f1)
            if [ "$major_version" -ge 8 ]; then
                return 0
            fi
        fi
    fi
    return 1
}

# Function to check Node.js version
check_node_version() {
    if command_exists node; then
        version=$(node --version 2>/dev/null)
        if [ $? -eq 0 ]; then
            major_version=$(echo $version | sed 's/v//' | cut -d. -f1)
            if [ "$major_version" -ge 18 ]; then
                return 0
            fi
        fi
    fi
    return 1
}

echo -e "${YELLOW}🔍 Checking system requirements...${NC}"

# Check .NET
dotnet_ok=false
if check_dotnet_version; then
    echo -e "${GREEN}✅ .NET 8.0+ is installed${NC}"
    dotnet_ok=true
else
    echo -e "${RED}❌ .NET 8.0+ is required${NC}"
fi

# Check Node.js
node_ok=false
if check_node_version; then
    echo -e "${GREEN}✅ Node.js 18+ is installed${NC}"
    node_ok=true
else
    echo -e "${RED}❌ Node.js 18+ is required${NC}"
fi

# Check npm
npm_ok=false
if command_exists npm; then
    echo -e "${GREEN}✅ npm is available${NC}"
    npm_ok=true
else
    echo -e "${RED}❌ npm is not available${NC}"
fi

echo ""

# If requirements not met, show installation instructions
if [ "$dotnet_ok" = false ] || [ "$node_ok" = false ] || [ "$npm_ok" = false ]; then
    echo -e "${RED}❌ Missing Requirements Detected!${NC}"
    echo ""
    echo -e "${YELLOW}Please install the following:${NC}"
    
    if [ "$dotnet_ok" = false ]; then
        echo -e "${WHITE}📥 .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0${NC}"
    fi
    
    if [ "$node_ok" = false ] || [ "$npm_ok" = false ]; then
        echo -e "${WHITE}📥 Node.js 18+: https://nodejs.org/${NC}"
    fi
    
    echo ""
    echo -e "${YELLOW}After installation, run this script again.${NC}"
    echo "Press any key to exit..."
    read -n 1
    exit 1
fi

echo -e "${GREEN}✅ All requirements met! Starting setup...${NC}"
echo ""

# Setup Backend
echo -e "${CYAN}🔧 Setting up Backend (.NET API)...${NC}"

echo -e "${YELLOW}   Restoring .NET packages...${NC}"
if dotnet restore >/dev/null 2>&1; then
    echo -e "${YELLOW}   Building backend...${NC}"
    if dotnet build >/dev/null 2>&1; then
        echo -e "${GREEN}✅ Backend setup complete!${NC}"
    else
        echo -e "${RED}❌ Backend build failed${NC}"
        exit 1
    fi
else
    echo -e "${RED}❌ Backend package restore failed${NC}"
    exit 1
fi

# Setup Frontend
echo ""
echo -e "${CYAN}🎨 Setting up Frontend (React App)...${NC}"

if [ -d "frontend" ]; then
    cd frontend
    
    echo -e "${YELLOW}   Installing Node.js packages (this may take a few minutes)...${NC}"
    if npm install >/dev/null 2>&1; then
        echo -e "${GREEN}✅ Frontend setup complete!${NC}"
    else
        echo -e "${RED}❌ Frontend setup failed${NC}"
        cd ..
        exit 1
    fi
    
    cd ..
else
    echo -e "${YELLOW}⚠️  Frontend folder not found. Skipping frontend setup.${NC}"
fi

# Run Tests
echo ""
echo -e "${CYAN}🧪 Running Tests...${NC}"

if dotnet test Tests/Tests.csproj --verbosity quiet >/dev/null 2>&1; then
    echo -e "${GREEN}✅ All tests passed!${NC}"
else
    echo -e "${YELLOW}⚠️  Some tests failed, but setup can continue.${NC}"
fi

echo ""
echo -e "${GREEN}🎉 Setup Complete!${NC}"
echo -e "${GREEN}==================${NC}"
echo ""
echo -e "${CYAN}To start the application:${NC}"
echo ""
echo -e "${YELLOW}1️⃣  Start Backend (in this terminal):${NC}"
echo -e "${WHITE}   cd API${NC}"
echo -e "${WHITE}   dotnet run${NC}"
echo ""
echo -e "${YELLOW}2️⃣  Start Frontend (in a NEW terminal):${NC}"
echo -e "${WHITE}   cd frontend${NC}"
echo -e "${WHITE}   npm run dev${NC}"
echo ""
echo -e "${YELLOW}3️⃣  Open your browser:${NC}"
echo -e "${WHITE}   Frontend: http://localhost:5173${NC}"
echo -e "${WHITE}   API Docs: https://localhost:7123/swagger${NC}"
echo ""

# Ask if user wants to start the backend now
echo -e -n "${CYAN}Would you like to start the backend server now? (y/n): ${NC}"
read response

if [ "$response" = "y" ] || [ "$response" = "Y" ] || [ "$response" = "yes" ]; then
    echo ""
    echo -e "${GREEN}🚀 Starting backend server...${NC}"
    echo -e "${YELLOW}   (Open a new terminal and run 'cd frontend && npm run dev' to start the frontend)${NC}"
    echo ""
    
    cd API
    dotnet run
else
    echo ""
    echo -e "${GREEN}👍 Setup complete! Follow the instructions above to start the servers.${NC}"
    echo ""
    echo -e "${CYAN}Need help? Check the README.md file for detailed instructions.${NC}"
fi