using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AssignedUser)
                  .WithMany(u => u.AssignedTasks)
                  .HasForeignKey(e => e.AssignedUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);

            entity.HasOne(e => e.Task)
                  .WithMany(t => t.Comments)
                  .HasForeignKey(e => e.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, FirstName = "Giorgi", LastName = "Beridze", Email = "giorgi.beridze@example.com" },
            new User { Id = 2, FirstName = "Nino", LastName = "Kapanadze", Email = "nino.kapanadze@example.com" }
        );

        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Name = "Website Redesign", Description = "Full redesign of the company website" },
            new Project { Id = 2, Name = "Mobile App", Description = "Cross-platform mobile application" }
        );

        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem { Id = 1, Title = "Design mockups", Description = "Create initial design mockups", Status = Core.Enums.TaskItemStatus.Done, ProjectId = 1, AssignedUserId = 1 },
            new TaskItem { Id = 2, Title = "Implement API", Description = "Build REST API endpoints", Status = Core.Enums.TaskItemStatus.InProgress, ProjectId = 1, AssignedUserId = 2 },
            new TaskItem { Id = 3, Title = "Setup CI/CD", Description = "Configure pipeline", Status = Core.Enums.TaskItemStatus.Todo, ProjectId = 2, AssignedUserId = null }
        );

        modelBuilder.Entity<Comment>().HasData(
            new Comment { Id = 1, Content = "Mockups approved by client.", TaskId = 1 },
            new Comment { Id = 2, Content = "Starting with authentication endpoints.", TaskId = 2 }
        );
    }
}
