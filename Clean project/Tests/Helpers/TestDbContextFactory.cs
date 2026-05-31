using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain.Entites;

namespace Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            
            // Seed test data
            SeedTestData(context);
            
            return context;
        }

        private static void SeedTestData(ApplicationDbContext context)
        {
            // Create test users
            var adminUser = new User
            {
                Id = 1,
                Username = "testadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Admin"
            };

            var regularUser = new User
            {
                Id = 2,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "User"
            };

            context.Users.AddRange(adminUser, regularUser);

            // Create test categories
            var workCategory = new Category
            {
                Id = 1,
                Name = "Work",
                Description = "Work related tasks",
                CreatedBy = adminUser.Id,
                CreatedDate = DateTime.UtcNow
            };

            var personalCategory = new Category
            {
                Id = 2,
                Name = "Personal",
                Description = "Personal tasks",
                CreatedBy = adminUser.Id,
                CreatedDate = DateTime.UtcNow
            };

            context.Categories.AddRange(workCategory, personalCategory);

            // Create test tasks
            var task1 = new TaskItem
            {
                Id = 1,
                Title = "Test Task 1",
                Description = "First test task",
                Status = "Pending",
                Priority = 5,
                DueDate = DateTime.UtcNow.AddDays(7),
                Category = "Work",
                UserId = regularUser.Id
            };

            var task2 = new TaskItem
            {
                Id = 2,
                Title = "Test Task 2",
                Description = "Second test task",
                Status = "InProgress",
                Priority = 8,
                DueDate = DateTime.UtcNow.AddDays(3),
                Category = "Personal",
                UserId = adminUser.Id
            };

            var task3 = new TaskItem
            {
                Id = 3,
                Title = "Test Task 3",
                Description = "Third test task",
                Status = "Completed",
                Priority = 3,
                DueDate = DateTime.UtcNow.AddDays(-1),
                Category = "Work",
                UserId = regularUser.Id
            };

            context.Tasks.AddRange(task1, task2, task3);
            context.SaveChanges();
        }
    }
}