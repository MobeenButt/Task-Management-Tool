# SonarQube Setup and Configuration

This document provides instructions for setting up SonarQube code quality analysis for the Task Management System.

## Prerequisites

1. **SonarQube Server**: You need a running SonarQube server instance
2. **SonarQube Scanner for .NET**: Command-line tool for analysis
3. **.NET SDK**: Version 8.0 or later

## SonarQube Server Setup

### Option 1: Docker (Recommended for Development)

```bash
# Pull and run SonarQube Community Edition
docker run -d --name sonarqube -p 9000:9000 sonarqube:community

# Wait for SonarQube to start (check http://localhost:9000)
# Default credentials: admin/admin (change on first login)
```

### Option 2: Manual Installation

1. Download SonarQube Community Edition from https://www.sonarqube.org/downloads/
2. Extract and run according to the installation guide
3. Access the web interface at http://localhost:9000

## SonarQube Scanner Installation

```powershell
# Install SonarQube Scanner for .NET globally
dotnet tool install --global dotnet-sonarscanner

# Verify installation
dotnet sonarscanner --version
```

## Project Configuration

### 1. Create SonarQube Project

1. Log into SonarQube web interface (http://localhost:9000)
2. Click "Create Project" → "Manually"
3. Set Project Key: `task-management-system`
4. Set Display Name: `Task Management System`
5. Generate a token for authentication

### 2. Update Configuration

Edit `run-sonar-analysis.ps1` and update these variables:

```powershell
$SONAR_HOST_URL = "http://localhost:9000"  # Your SonarQube URL
$SONAR_TOKEN = "your-actual-token-here"    # Token from step 1
```

## Running Analysis

### Automated Analysis (Recommended)

```powershell
# Run the complete analysis script
./run-sonar-analysis.ps1
```

### Manual Analysis Steps

```powershell
# 1. Clean the solution
dotnet clean

# 2. Begin SonarQube analysis
dotnet sonarscanner begin /k:"task-management-system" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="your-token"

# 3. Build the solution
dotnet build

# 4. Run tests with coverage
dotnet test Tests/Tests.csproj --collect:"XPlat Code Coverage" --results-directory Tests/TestResults --logger trx

# 5. End analysis and upload results
dotnet sonarscanner end /d:sonar.login="your-token"
```

## Quality Gates and Rules

The project is configured with the following quality standards:

### Code Coverage
- **Target**: Minimum 80% line coverage
- **Critical**: No uncovered critical paths

### Code Smells
- **Maintainability Rating**: A (≤5% technical debt ratio)
- **Cyclomatic Complexity**: ≤15 per method
- **Cognitive Complexity**: ≤15 per method

### Security
- **Security Rating**: A (no security hotspots)
- **Security Review Rating**: A (all security hotspots reviewed)

### Reliability
- **Reliability Rating**: A (no bugs)
- **Duplicated Lines**: <3%

### Custom Rules for C#

The analysis includes these specific C# rules:

- **Naming Conventions**: PascalCase for public members, camelCase for private
- **Exception Handling**: Proper exception handling and logging
- **SOLID Principles**: Dependency injection and single responsibility
- **Security**: SQL injection prevention, input validation

## Exclusions

The following files/folders are excluded from analysis:

- `**/bin/**` - Build output
- `**/obj/**` - Build artifacts  
- `**/wwwroot/**` - Static web assets
- `**/*.min.js` - Minified JavaScript
- `**/*.min.css` - Minified CSS
- `**/Program.cs` - Entry point (minimal logic)
- `**/*Dto.cs` - Data transfer objects
- `**/Migrations/**` - Entity Framework migrations

## Viewing Results

After analysis completion:

1. Open SonarQube dashboard: http://localhost:9000
2. Navigate to your project: `task-management-system`
3. Review the quality gate status and metrics
4. Address any issues found in the "Issues" tab

## Integration with CI/CD

For continuous integration, add SonarQube analysis to your build pipeline:

```yaml
# Example GitHub Actions workflow
- name: SonarQube Analysis
  run: |
    dotnet sonarscanner begin /k:"task-management-system" /d:sonar.host.url="${{ secrets.SONAR_HOST_URL }}" /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
    dotnet build
    dotnet test --collect:"XPlat Code Coverage"
    dotnet sonarscanner end /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
```

## Troubleshooting

### Common Issues

1. **Scanner not found**: Ensure `dotnet-sonarscanner` is installed globally
2. **Connection refused**: Verify SonarQube server is running on correct port
3. **Authentication failed**: Check token validity and permissions
4. **No coverage data**: Ensure tests run successfully and generate coverage reports

### Logs and Debugging

- SonarQube logs: Check server logs in SonarQube installation directory
- Scanner logs: Available in console output during analysis
- Coverage reports: Located in `Tests/TestResults/` directory

## Quality Metrics Dashboard

Key metrics to monitor:

- **Lines of Code**: Total codebase size
- **Coverage**: Percentage of code covered by tests
- **Duplications**: Percentage of duplicated code
- **Maintainability**: Technical debt and code smells
- **Reliability**: Bugs and potential issues
- **Security**: Vulnerabilities and security hotspots

## Best Practices

1. **Run analysis regularly**: Before each commit or pull request
2. **Fix issues promptly**: Address new issues before they accumulate
3. **Monitor trends**: Track quality metrics over time
4. **Set quality gates**: Fail builds that don't meet quality standards
5. **Review security hotspots**: Manually review all security-related findings