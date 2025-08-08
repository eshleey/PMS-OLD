using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemAPI.Models;

namespace ProjectManagementSystemAPI.Data
{
    public class AppDbContext :DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
