using API.Properties.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Tests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TaskController(_mockTaskService.Object);
            
            // Setup user context
            SetupUserContext("1", "testuser", "User");
        }

        private void SetupUserContext(string userId, string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Fact]
        public void GetTasks_AsRegularUser_ShouldReturnUserTasks()
        {
            // Arrange
            var expectedTasks = new List<TaskResponseDto>
            {
                new TaskResponseDto { Id = 1, Title = "Task 1", UserId = 1 },
                new TaskResponseDto { Id = 2, Title = "Task 2", UserId = 1 }
            };
            _mockTaskService.Setup(s => s.GetAllByUserId(1)).Returns(expectedTasks);

            // Act
            var result = _controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tasks = Assert.IsAssignableFrom<List<TaskResponseDto>>(okResult.Value);
            Assert.Equal(2, tasks.Count);
            _mockTaskService.Verify(s => s.GetAllByUserId(1), Times.Once);
        }

        [Fact]
        public void GetTasks_AsAdmin_ShouldReturnAllTasks()
        {
            // Arrange
            SetupUserContext("1", "admin", "Admin");
            var expectedTasks = new List<TaskResponseDto>
            {
                new TaskResponseDto { Id = 1, Title = "Task 1", UserId = 1 },
                new TaskResponseDto { Id = 2, Title = "Task 2", UserId = 2 },
                new TaskResponseDto { Id = 3, Title = "Task 3", UserId = 3 }
            };
            _mockTaskService.Setup(s => s.GetAllTasks()).Returns(expectedTasks);

            // Act
            var result = _controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tasks = Assert.IsAssignableFrom<List<TaskResponseDto>>(okResult.Value);
            Assert.Equal(3, tasks.Count);
            _mockTaskService.Verify(s => s.GetAllTasks(), Times.Once);
        }

        [Fact]
        public void GetTask_WithValidId_ShouldReturnTask()
        {
            // Arrange
            int taskId = 1;
            var expectedTask = new TaskResponseDto { Id = taskId, Title = "Test Task", UserId = 1 };
            _mockTaskService.Setup(s => s.GetById(taskId, 1)).Returns(expectedTask);

            // Act
            var result = _controller.GetTask(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var task = Assert.IsType<TaskResponseDto>(okResult.Value);
            Assert.Equal(taskId, task.Id);
        }

        [Fact]
        public void CreateTask_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var createDto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "Task description",
                Status = "Pending",
                Priority = 5,
                DueDate = DateTime.UtcNow.AddDays(7),
                Category = "Work"
            };

            // Act
            var result = _controller.CreateTask(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockTaskService.Verify(s => s.Create(createDto, 1), Times.Once);
        }

        [Fact]
        public void UpdateTask_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            int taskId = 1;
            var updateDto = new UpdateTaskDto
            {
                Title = "Updated Task",
                Description = "Updated description",
                Status = "InProgress",
                Priority = 8,
                DueDate = DateTime.UtcNow.AddDays(5),
                Category = "Personal"
            };

            // Act
            var result = _controller.UpdateTask(taskId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(taskId, updateDto.Id);
            _mockTaskService.Verify(s => s.Update(updateDto, 1), Times.Once);
        }

        [Fact]
        public void DeleteTask_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            int taskId = 1;

            // Act
            var result = _controller.DeleteTask(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockTaskService.Verify(s => s.Delete(taskId, 1), Times.Once);
        }

        [Fact]
        public void GetDashboard_AsRegularUser_ShouldReturnUserStats()
        {
            // Arrange
            var expectedStats = new Dictionary<string, int>
            {
                { "Pending", 2 },
                { "InProgress", 1 },
                { "Completed", 3 }
            };
            _mockTaskService.Setup(s => s.GetTaskCountByStatus(1)).Returns(expectedStats);

            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = Assert.IsAssignableFrom<Dictionary<string, int>>(okResult.Value);
            Assert.Equal(expectedStats, stats);
            _mockTaskService.Verify(s => s.GetTaskCountByStatus(1), Times.Once);
        }

        [Fact]
        public void GetDashboard_AsAdmin_ShouldReturnAllStats()
        {
            // Arrange
            SetupUserContext("1", "admin", "Admin");
            var expectedStats = new Dictionary<string, int>
            {
                { "Pending", 5 },
                { "InProgress", 3 },
                { "Completed", 7 }
            };
            _mockTaskService.Setup(s => s.GetTaskCountByStatus(null)).Returns(expectedStats);

            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = Assert.IsAssignableFrom<Dictionary<string, int>>(okResult.Value);
            Assert.Equal(expectedStats, stats);
            _mockTaskService.Verify(s => s.GetTaskCountByStatus(null), Times.Once);
        }

        [Fact]
        public void ReassignTask_AsAdmin_ShouldReturnSuccess()
        {
            // Arrange
            SetupUserContext("1", "admin", "Admin");
            int taskId = 1;
            var reassignDto = new TaskController.ReassignTaskDto { NewUserId = 2 };

            // Act
            var result = _controller.ReassignTask(taskId, reassignDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockTaskService.Verify(s => s.ReassignTask(taskId, 2), Times.Once);
        }

        [Fact]
        public void GetTasks_WhenServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            _mockTaskService.Setup(s => s.GetAllByUserId(It.IsAny<int>()))
                           .Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetTasks();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}