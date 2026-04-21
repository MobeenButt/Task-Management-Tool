using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using TaskManagement.Domain;

namespace TaskManagement.Infrastructure;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { 
    }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
}
