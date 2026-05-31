# Code Quality Fix Script
# This script addresses the compiler warnings and code quality issues identified

Write-Host "Starting Code Quality Fixes..." -ForegroundColor Green

# Function to backup files before modification
function Backup-File {
    param($FilePath)
    $backupPath = "$FilePath.backup"
    if (Test-Path $FilePath) {
        Copy-Item $FilePath $backupPath
        Write-Host "Backed up: $FilePath" -ForegroundColor Yellow
    }
}

# Function to restore files if needed
function Restore-Files {
    Write-Host "Restoring backup files..." -ForegroundColor Yellow
    Get-ChildItem -Recurse -Filter "*.backup" | ForEach-Object {
        $originalPath = $_.FullName -replace '\.backup$', ''
        Move-Item $_.FullName $originalPath -Force
        Write-Host "Restored: $originalPath" -ForegroundColor Green
    }
}

try {
    Write-Host "Fixing Code Quality Issues..." -ForegroundColor Cyan

    # 1. Fix TaskItem.cs - Add required modifier
    Write-Host "1. Fixing TaskItem.cs null reference warning..." -ForegroundColor Yellow
    $taskItemPath = "Domain\Entites\TaskItem.cs"
    if (Test-Path $taskItemPath) {
        Backup-File $taskItemPath
        $content = Get-Content $taskItemPath -Raw
        $content = $content -replace 'public string\? Status { get; set; }', 'public required string Status { get; set; }'
        Set-Content $taskItemPath $content -NoNewline
        Write-Host "   ✓ Fixed TaskItem Status property" -ForegroundColor Green
    }

    # 2. Add validation attributes to DTOs
    Write-Host "2. Adding validation attributes to DTOs..." -ForegroundColor Yellow
    
    # Fix CreateTaskDto
    $createTaskDtoPath = "Application\DTOs\TaskDtos.cs"
    if (Test-Path $createTaskDtoPath) {
        Backup-File $createTaskDtoPath
        $content = Get-Content $createTaskDtoPath -Raw
        
        # Add using statement for validation
        if ($content -notmatch 'using System.ComponentModel.DataAnnotations;') {
            $content = $content -replace 'using System;', "using System;`nusing System.ComponentModel.DataAnnotations;"
        }
        
        # Add validation attributes
        $content = $content -replace 'public string\? Title { get; set; }', '[Required][StringLength(100, MinimumLength = 1)]public string? Title { get; set; }'
        $content = $content -replace 'public string\? Description { get; set; }', '[StringLength(500)]public string? Description { get; set; }'
        $content = $content -replace 'public string\? Status { get; set; }', '[Required][RegularExpression("^(Pending|InProgress|Completed)$")]public string? Status { get; set; }'
        $content = $content -replace 'public string\? Category { get; set; }', '[Required][StringLength(50)]public string? Category { get; set; }'
        
        Set-Content $createTaskDtoPath $content -NoNewline
        Write-Host "   ✓ Added validation to CreateTaskDto" -ForegroundColor Green
    }

    # 3. Create custom exception classes
    Write-Host "3. Creating custom exception classes..." -ForegroundColor Yellow
    
    $exceptionsDir = "Domain\Exceptions"
    if (!(Test-Path $exceptionsDir)) {
        New-Item -ItemType Directory -Path $exceptionsDir -Force
    }

    # UserNotFoundException
    $userNotFoundPath = "$exceptionsDir\UserNotFoundException.cs"
    $userNotFoundContent = @"
using System;

namespace Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
        public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
"@
    Set-Content $userNotFoundPath $userNotFoundContent
    Write-Host "   ✓ Created UserNotFoundException" -ForegroundColor Green

    # TaskNotFoundException
    $taskNotFoundPath = "$exceptionsDir\TaskNotFoundException.cs"
    $taskNotFoundContent = @"
using System;

namespace Domain.Exceptions
{
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(string message) : base(message) { }
        public TaskNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
"@
    Set-Content $taskNotFoundPath $taskNotFoundContent
    Write-Host "   ✓ Created TaskNotFoundException" -ForegroundColor Green

    # CategoryNotFoundException
    $categoryNotFoundPath = "$exceptionsDir\CategoryNotFoundException.cs"
    $categoryNotFoundContent = @"
using System;

namespace Domain.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException(string message) : base(message) { }
        public CategoryNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
"@
    Set-Content $categoryNotFoundPath $categoryNotFoundContent
    Write-Host "   ✓ Created CategoryNotFoundException" -ForegroundColor Green

    Write-Host "`nCode Quality Fixes Applied Successfully!" -ForegroundColor Green
    Write-Host "`nNext Steps:" -ForegroundColor Cyan
    Write-Host "1. Update service classes to use custom exceptions" -ForegroundColor White
    Write-Host "2. Add null checks in service methods" -ForegroundColor White
    Write-Host "3. Test the application to ensure fixes work correctly" -ForegroundColor White
    Write-Host "4. Run 'dotnet build' to verify no compilation errors" -ForegroundColor White
    
    Write-Host "`nTo undo changes, run: Restore-Files" -ForegroundColor Yellow

} catch {
    Write-Host "Error occurred during fixes: $_" -ForegroundColor Red
    Write-Host "Restoring backup files..." -ForegroundColor Yellow
    Restore-Files
}