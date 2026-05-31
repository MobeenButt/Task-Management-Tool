using Application.DTOs;
using Application.Interfaces;
using Infrastructure.TaskServices;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;
using Xunit;

namespace Tests.Services
{
    public class TaskServiceTests : IDisposable
    {
        private readonly Infrastructure.ApplicationDbContext _context;
        private readonly ITaskService _taskService;

        public TaskServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _taskService = new TaskService(_context);
        }

        [Fact]
        public void GetAllTasks_ShouldReturnAllTasks()
        {
            // Act
            var result = _taskService.GetAllTasks();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, t => t.Title == "Test Task 1");
            Assert.Contains(result, t => t.Title == "Test Task 2");
            Assert.Contains(result, t => t.Title == "Test Task 3");
        }

        [Fact]
        public void GetAllByUserId_ShouldReturnUserSpecificTasks()
        {
            // Arrange
            int userId = 2; // regularUser

            // Act
            var result = _taskService.GetAllByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, task => Assert.Equal(userId, task.UserId));
        }

        [Fact]
        public void GetById_WithValidIdAndUser_ShouldReturnTask()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;

            // Act
            var result = _taskService.GetById(taskId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal("Test Task 1", result.Title);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public void GetById_WithInvalidUser_ShouldThrowException()
        {
            // Arrange
            int taskId = 1;
            int wrongUserId = 999;

            // Act & Assert
            Assert.Throws<Exception>(() => _taskService.GetById(taskId, wrongUserId));
        }

        [Fact]
        public void Create_WithValidData_ShouldCreateTask()
        {
            // Arrange
            var createDto = new CreateTaskDto
            {
                Title = "New Test Task",
                Description = "New task description",
                Status = "Pending",
                Priority = 7,
                DueDate = DateTime.UtcNow.AddDays(5),
                Category = "Work"
            };
            int userId = 2;

            // Act
            _taskService.Create(createDto, userId);

            // Assert
            var allTasks = _taskService.GetAllTasks();
            Assert.Equal(4, allTasks.Count);
            Assert.Contains(allTasks, t => t.Title == "New Test Task");
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateTask()
        {
            // Arrange
            var updateDto = new UpdateTaskDto
            {
                Id = 1,
                Title = "Updated Task Title",
                Description = "Updated description",
                Status = "InProgress",
                Priority = 9,
                DueDate = DateTime.UtcNow.AddDays(10),
                Category = "Personal"
            };
            int userId = 2;

            // Act
            _taskService.Update(updateDto, userId);

            // Assert
            var updatedTask = _taskService.GetById(1, userId);
            Assert.Equal("Updated Task Title", updatedTask.Title);
            Assert.Equal("Updated description", updatedTask.Description);
            Assert.Equal("InProgress", updatedTask.Status);
            Assert.Equal(9, updatedTask.Priority);
        }

        [Fact]
        public void Delete_WithValidId_ShouldSoftDeleteTask()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;

            // Act
            _taskService.Delete(taskId, userId);

            // Assert
            Assert.Throws<Exception>(() => _taskService.GetById(taskId, userId));
            
            // Verify task still exists in database but marked as deleted
            var taskInDb = _context.Tasks.IgnoreQueryFilters().FirstOrDefault(t => t.Id == taskId);
            Assert.NotNull(taskInDb);
            Assert.True(taskInDb.IsDeleted);
        }

        [Fact]
        public void GetTaskCountByStatus_WithoutUserId_ShouldReturnAllTaskCounts()
        {
            // Act
            var result = _taskService.GetTaskCountByStatus();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result["Pending"]);
            Assert.Equal(1, result["InProgress"]);
            Assert.Equal(1, result["Completed"]);
        }

        [Fact]
        public void GetTaskCountByStatus_WithUserId_ShouldReturnUserTaskCounts()
        {
            // Arrange
            int userId = 2; // regularUser has 2 tasks

            // Act
            var result = _taskService.GetTaskCountByStatus(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result["Pending"]);
            Assert.Equal(0, result["InProgress"]);
            Assert.Equal(1, result["Completed"]);
        }

        [Fact]
        public void ReassignTask_WithValidData_ShouldReassignTask()
        {
            // Arrange
            int taskId = 1;
            int newUserId = 1; // admin user

            // Act
            _taskService.ReassignTask(taskId, newUserId);

            // Assert
            var reassignedTask = _taskService.GetAllTasks().FirstOrDefault(t => t.Id == taskId);
            Assert.NotNull(reassignedTask);
            Assert.Equal(newUserId, reassignedTask.UserId);
        }

        [Fact]
        public void ReassignTask_WithInvalidTaskId_ShouldThrowException()
        {
            // Arrange
            int invalidTaskId = 999;
            int newUserId = 1;

            // Act & Assert
            Assert.Throws<Exception>(() => _taskService.ReassignTask(invalidTaskId, newUserId));
        }

        [Fact]
        public void ReassignTask_WithInvalidUserId_ShouldThrowException()
        {
            // Arrange
            int taskId = 1;
            int invalidUserId = 999;

            // Act & Assert
            Assert.Throws<Exception>(() => _taskService.ReassignTask(taskId, invalidUserId));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}