using Domain.Entites;

namespace Infrastructure
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Check if we already have data
            if (context.Tasks.Any() || context.Categories.Any())
            {
                return; // Data already seeded
            }

            // Get admin user
            var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
            if (admin == null) return;

            // Create some test users
            var testUsers = new List<User>();
            
            if (!context.Users.Any(u => u.Username == "john_doe"))
            {
                testUsers.Add(new User
                {
                    Username = "john_doe",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = "User"
                });
            }

            if (!context.Users.Any(u => u.Username == "jane_smith"))
            {
                testUsers.Add(new User
                {
                    Username = "jane_smith",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = "User"
                });
            }

            if (testUsers.Any())
            {
                context.Users.AddRange(testUsers);
                context.SaveChanges();
            }

            // Get all users for task assignment
            var allUsers = context.Users.ToList();

            // Seed Categories
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Work",
                    Description = "Work-related tasks",
                    CreatedBy = admin.Id,
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Personal",
                    Description = "Personal tasks and errands",
                    CreatedBy = admin.Id,
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Shopping",
                    Description = "Shopping and purchases",
                    CreatedBy = admin.Id,
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Health",
                    Description = "Health and fitness tasks",
                    CreatedBy = admin.Id,
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Learning",
                    Description = "Learning and development",
                    CreatedBy = admin.Id,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Seed Sample Tasks with different users
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Title = "Complete Project Documentation",
                    Description = "Write comprehensive documentation for the task management system",
                    Status = "InProgress",
                    Priority = 8,
                    DueDate = DateTime.UtcNow.AddDays(3),
                    Category = "Work",
                    UserId = admin.Id
                },
                new TaskItem
                {
                    Title = "Review Code Changes",
                    Description = "Review pull requests and provide feedback to team members",
                    Status = "Pending",
                    Priority = 7,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Category = "Work",
                    UserId = allUsers.FirstOrDefault(u => u.Username == "john_doe")?.Id ?? admin.Id
                },
                new TaskItem
                {
                    Title = "Team Meeting Preparation",
                    Description = "Prepare slides and agenda for weekly team meeting",
                    Status = "Pending",
                    Priority = 6,
                    DueDate = DateTime.UtcNow.AddDays(2),
                    Category = "Work",
                    UserId = allUsers.FirstOrDefault(u => u.Username == "jane_smith")?.Id ?? admin.Id
                },
                new TaskItem
                {
                    Title = "Grocery Shopping",
                    Description = "Buy groceries for the week including fruits, vegetables, and dairy",
                    Status = "Pending",
                    Priority = 5,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Category = "Shopping",
                    UserId = allUsers.FirstOrDefault(u => u.Username == "john_doe")?.Id ?? admin.Id
                },
                new TaskItem
                {
                    Title = "Gym Workout",
                    Description = "Complete full body workout routine at the gym",
                    Status = "Completed",
                    Priority = 7,
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    Category = "Health",
                    UserId = admin.Id
                },
                new TaskItem
                {
                    Title = "Learn React Hooks",
                    Description = "Complete online course on advanced React hooks patterns",
                    Status = "InProgress",
                    Priority = 8,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    Category = "Learning",
                    UserId = allUsers.FirstOrDefault(u => u.Username == "jane_smith")?.Id ?? admin.Id
                },
                new TaskItem
                {
                    Title = "Pay Utility Bills",
                    Description = "Pay electricity, water, and internet bills for the month",
                    Status = "Completed",
                    Priority = 9,
                    DueDate = DateTime.UtcNow.AddDays(-2),
                    Category = "Personal",
                    UserId = admin.Id
                },
                new TaskItem
                {
                    Title = "Database Optimization",
                    Description = "Optimize database queries and add proper indexes",
                    Status = "Pending",
                    Priority = 9,
                    DueDate = DateTime.UtcNow.AddDays(5),
                    Category = "Work",
                    UserId = allUsers.FirstOrDefault(u => u.Username == "john_doe")?.Id ?? admin.Id
                },
                new TaskItem
                {
                    Title = "Buy Birthday Gift",
                    Description = "Find and purchase birthday gift for friend",
                    Status = "InProgress",
                    Priority = 6,
                    DueDate = DateTime.UtcNow.AddDays(4),
                    Category = "Shopping",
                    UserId = allUsers.FirstOrDefault(u => u.Username == "jane_smith")?.Id ?? admin.Id
                },
                new TaskItem
                {
                    Title = "Annual Health Checkup",
                    Description = "Schedule and complete annual health checkup with doctor",
                    Status = "Pending",
                    Priority = 8,
                    DueDate = DateTime.UtcNow.AddDays(10),
                    Category = "Health",
                    UserId = admin.Id
                }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
        }
    }
}
