# Task Management System - Automated Setup Script
# This script helps you set up and run the Task Management System quickly

Write-Host "🚀 Task Management System - Quick Setup" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

# Function to check if a command exists
function Test-Command {
    param($Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    } catch {
        return $false
    }
}

# Function to check .NET version
function Test-DotNetVersion {
    try {
        $version = dotnet --version
        $majorVersion = [int]($version.Split('.')[0])
        return $majorVersion -ge 8
    } catch {
        return $false
    }
}

# Function to check Node.js version
function Test-NodeVersion {
    try {
        $version = node --version
        $majorVersion = [int]($version.Substring(1).Split('.')[0])
        return $majorVersion -ge 18
    } catch {
        return $false
    }
}

Write-Host "🔍 Checking system requirements..." -ForegroundColor Yellow

# Check .NET
if (Test-Command "dotnet") {
    if (Test-DotNetVersion) {
        Write-Host "✅ .NET 8.0+ is installed" -ForegroundColor Green
        $dotnetOk = $true
    } else {
        Write-Host "❌ .NET 8.0+ is required (found older version)" -ForegroundColor Red
        $dotnetOk = $false
    }
} else {
    Write-Host "❌ .NET is not installed" -ForegroundColor Red
    $dotnetOk = $false
}

# Check Node.js
if (Test-Command "node") {
    if (Test-NodeVersion) {
        Write-Host "✅ Node.js 18+ is installed" -ForegroundColor Green
        $nodeOk = $true
    } else {
        Write-Host "❌ Node.js 18+ is required (found older version)" -ForegroundColor Red
        $nodeOk = $false
    }
} else {
    Write-Host "❌ Node.js is not installed" -ForegroundColor Red
    $nodeOk = $false
}

# Check npm
if (Test-Command "npm") {
    Write-Host "✅ npm is available" -ForegroundColor Green
    $npmOk = $true
} else {
    Write-Host "❌ npm is not available" -ForegroundColor Red
    $npmOk = $false
}

Write-Host ""

# If requirements not met, show installation instructions
if (-not ($dotnetOk -and $nodeOk -and $npmOk)) {
    Write-Host "❌ Missing Requirements Detected!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install the following:" -ForegroundColor Yellow
    
    if (-not $dotnetOk) {
        Write-Host "📥 .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
    }
    
    if (-not $nodeOk -or -not $npmOk) {
        Write-Host "📥 Node.js 18+: https://nodejs.org/" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "After installation, run this script again." -ForegroundColor Yellow
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "✅ All requirements met! Starting setup..." -ForegroundColor Green
Write-Host ""

# Setup Backend
Write-Host "🔧 Setting up Backend (.NET API)..." -ForegroundColor Cyan

try {
    Write-Host "   Restoring .NET packages..." -ForegroundColor Yellow
    dotnet restore | Out-Null
    
    Write-Host "   Building backend..." -ForegroundColor Yellow
    dotnet build | Out-Null
    
    Write-Host "✅ Backend setup complete!" -ForegroundColor Green
} catch {
    Write-Host "❌ Backend setup failed: $_" -ForegroundColor Red
    exit 1
}

# Setup Frontend
Write-Host ""
Write-Host "🎨 Setting up Frontend (React App)..." -ForegroundColor Cyan

if (Test-Path "frontend") {
    try {
        Push-Location "frontend"
        
        Write-Host "   Installing Node.js packages (this may take a few minutes)..." -ForegroundColor Yellow
        npm install | Out-Null
        
        Write-Host "✅ Frontend setup complete!" -ForegroundColor Green
        
        Pop-Location
    } catch {
        Write-Host "❌ Frontend setup failed: $_" -ForegroundColor Red
        Pop-Location
        exit 1
    }
} else {
    Write-Host "⚠️  Frontend folder not found. Skipping frontend setup." -ForegroundColor Yellow
}

# Run Tests
Write-Host ""
Write-Host "🧪 Running Tests..." -ForegroundColor Cyan

try {
    $testResult = dotnet test Tests/Tests.csproj --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ All tests passed!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Some tests failed, but setup can continue." -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️  Could not run tests, but setup can continue." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🎉 Setup Complete!" -ForegroundColor Green
Write-Host "==================" -ForegroundColor Green
Write-Host ""
Write-Host "To start the application:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1️⃣  Start Backend (in this terminal):" -ForegroundColor Yellow
Write-Host "   cd API" -ForegroundColor White
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "2️⃣  Start Frontend (in a NEW terminal):" -ForegroundColor Yellow
Write-Host "   cd frontend" -ForegroundColor White
Write-Host "   npm run dev" -ForegroundColor White
Write-Host ""
Write-Host "3️⃣  Open your browser:" -ForegroundColor Yellow
Write-Host "   Frontend: http://localhost:5173" -ForegroundColor White
Write-Host "   API Docs: https://localhost:7123/swagger" -ForegroundColor White
Write-Host ""

# Ask if user wants to start the backend now
Write-Host "Would you like to start the backend server now? (y/n): " -ForegroundColor Cyan -NoNewline
$response = Read-Host

if ($response -eq 'y' -or $response -eq 'Y' -or $response -eq 'yes') {
    Write-Host ""
    Write-Host "🚀 Starting backend server..." -ForegroundColor Green
    Write-Host "   (Open a new terminal and run 'cd frontend && npm run dev' to start the frontend)" -ForegroundColor Yellow
    Write-Host ""
    
    Set-Location "API"
    dotnet run
} else {
    Write-Host ""
    Write-Host "👍 Setup complete! Follow the instructions above to start the servers." -ForegroundColor Green
    Write-Host ""
    Write-Host "Need help? Check the README.md file for detailed instructions." -ForegroundColor Cyan
}