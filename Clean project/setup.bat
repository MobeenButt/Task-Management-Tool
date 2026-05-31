@echo off
title Task Management System - Quick Setup

echo.
echo ========================================
echo  Task Management System - Quick Setup
echo ========================================
echo.

REM Check if PowerShell is available
powershell -Command "Write-Host 'PowerShell is available'" >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: PowerShell is required but not found.
    echo Please install PowerShell or run setup.ps1 manually.
    pause
    exit /b 1
)

echo Running PowerShell setup script...
echo.

REM Run the PowerShell setup script
powershell -ExecutionPolicy Bypass -File "setup.ps1"

echo.
echo Setup script completed.
pause