# SonarQube Analysis Script for Task Management System
# This script runs SonarQube analysis on the .NET project

Write-Host "Starting SonarQube Analysis for Task Management System" -ForegroundColor Green

# Check if SonarQube Scanner is installed
try {
    Write-Host "Checking SonarQube Scanner installation..." -ForegroundColor Yellow
    $scannerCheck = dotnet dotnet-sonarscanner --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "SonarQube Scanner not found locally. Installing..." -ForegroundColor Yellow
        dotnet tool install dotnet-sonarscanner
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to install SonarQube Scanner. Please install manually." -ForegroundColor Red
            Write-Host "Run: dotnet tool install dotnet-sonarscanner" -ForegroundColor Yellow
            exit 1
        }
    } else {
        Write-Host "SonarQube Scanner found!" -ForegroundColor Green
    }
} catch {
    Write-Host "Error checking SonarQube Scanner installation: $_" -ForegroundColor Red
    exit 1
}

# SonarQube server configuration (update these values for your SonarQube instance)
$SONAR_HOST_URL = "http://localhost:9000"  # Default SonarQube URL
$SONAR_TOKEN = "your-sonar-token-here"     # Replace with your actual token
$PROJECT_KEY = "task-management-system"

Write-Host "Configuration:" -ForegroundColor Cyan
Write-Host "  SonarQube URL: $SONAR_HOST_URL" -ForegroundColor White
Write-Host "  Project Key: $PROJECT_KEY" -ForegroundColor White

# Check if SonarQube server is accessible
Write-Host "Checking SonarQube server connectivity..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $SONAR_HOST_URL -Method GET -TimeoutSec 5 -ErrorAction Stop
    Write-Host "SonarQube server is accessible!" -ForegroundColor Green
    $serverAvailable = $true
} catch {
    Write-Host "SonarQube server is not accessible at $SONAR_HOST_URL" -ForegroundColor Yellow
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Continuing with local analysis only..." -ForegroundColor Yellow
    $serverAvailable = $false
}

# Clean previous build artifacts
Write-Host "Cleaning previous build artifacts..." -ForegroundColor Yellow
dotnet clean

if ($serverAvailable) {
    # Begin SonarQube analysis (only if server is available)
    Write-Host "Starting SonarQube analysis..." -ForegroundColor Yellow
    dotnet dotnet-sonarscanner begin `
        /k:$PROJECT_KEY `
        /d:sonar.host.url=$SONAR_HOST_URL `
        /d:sonar.login=$SONAR_TOKEN `
        /d:sonar.cs.opencover.reportsPaths="Tests/TestResults/**/coverage.opencover.xml" `
        /d:sonar.cs.vstest.reportsPaths="Tests/TestResults/**/*.trx"

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to start SonarQube analysis. Check your configuration." -ForegroundColor Red
        Write-Host "Continuing with local build and test only..." -ForegroundColor Yellow
        $serverAvailable = $false
    }
} else {
    Write-Host "Skipping SonarQube server analysis - running local build and tests only" -ForegroundColor Yellow
}

# Build the solution
Write-Host "Building the solution..." -ForegroundColor Yellow
dotnet build --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Please fix build errors before running analysis." -ForegroundColor Red
    exit 1
}

# Run tests with coverage
Write-Host "Running tests with coverage..." -ForegroundColor Yellow
dotnet test Tests/Tests.csproj `
    --no-build `
    --verbosity normal `
    --collect:"XPlat Code Coverage" `
    --results-directory Tests/TestResults `
    --logger trx

if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed. Please fix test failures before completing analysis." -ForegroundColor Red
}

# End SonarQube analysis (only if server analysis was started)
if ($serverAvailable) {
    Write-Host "Completing SonarQube analysis..." -ForegroundColor Yellow
    dotnet dotnet-sonarscanner end /d:sonar.login=$SONAR_TOKEN

    if ($LASTEXITCODE -eq 0) {
        Write-Host "SonarQube analysis completed successfully!" -ForegroundColor Green
        Write-Host "Check your SonarQube dashboard at: $SONAR_HOST_URL" -ForegroundColor Cyan
    } else {
        Write-Host "SonarQube analysis failed. Check the logs above for details." -ForegroundColor Red
    }
} else {
    Write-Host "Local analysis completed!" -ForegroundColor Green
    Write-Host "To run full SonarQube analysis:" -ForegroundColor Cyan
    Write-Host "1. Start SonarQube server: docker run -d --name sonarqube -p 9000:9000 sonarqube:community" -ForegroundColor White
    Write-Host "2. Create project and get token at http://localhost:9000" -ForegroundColor White
    Write-Host "3. Update SONAR_TOKEN in this script and run again" -ForegroundColor White
}