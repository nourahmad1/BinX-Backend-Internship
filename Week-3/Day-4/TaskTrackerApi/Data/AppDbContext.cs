using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Entities;

namespace TaskTrackerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }


    public DbSet<User> Users => Set<User>();


    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}