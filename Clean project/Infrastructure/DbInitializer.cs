using Domain.Entites;

namespace Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Check database is created
            context.Database.EnsureCreated();
            // Check if admin already exists
            if (context.Users.Any(u=>u.Username=="admin"))
            {
                return; // Admin Already exists
            }
            // fixed admin user
            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            var admin = new User
            {
                Username = "admin",
                PasswordHash = adminPasswordHash,
                Role = "Admin"
            };
            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}
