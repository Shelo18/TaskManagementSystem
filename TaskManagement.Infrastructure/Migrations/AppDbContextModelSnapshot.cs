using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using TaskManagement.Infrastructure.Data;

#nullable disable

namespace TaskManagement.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TaskManagement.Core.Entities.Comment", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd()
                    .HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                b.Property<string>("Content").IsRequired()
                    .HasMaxLength(1000).HasColumnType("nvarchar(1000)");
                b.Property<int>("TaskId").HasColumnType("int");
                b.HasKey("Id");
                b.HasIndex("TaskId");
                b.ToTable("Comments");
                b.HasData(
                    new { Id = 1, Content = "Mockups approved by client.", TaskId = 1 },
                    new { Id = 2, Content = "Starting with authentication endpoints.", TaskId = 2 });
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.Project", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd()
                    .HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                b.Property<string>("Description").HasMaxLength(500).HasColumnType("nvarchar(500)");
                b.Property<string>("Name").IsRequired()
                    .HasMaxLength(100).HasColumnType("nvarchar(100)");
                b.HasKey("Id");
                b.ToTable("Projects");
                b.HasData(
                    new { Id = 1, Name = "Website Redesign", Description = "Full redesign of the company website" },
                    new { Id = 2, Name = "Mobile App", Description = "Cross-platform mobile application" });
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.TaskItem", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd()
                    .HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                b.Property<int?>("AssignedUserId").HasColumnType("int");
                b.Property<string>("Description").HasColumnType("nvarchar(max)");
                b.Property<int>("ProjectId").HasColumnType("int");
                b.Property<int>("Status").HasColumnType("int");
                b.Property<string>("Title").IsRequired()
                    .HasMaxLength(200).HasColumnType("nvarchar(200)");
                b.HasKey("Id");
                b.HasIndex("AssignedUserId");
                b.HasIndex("ProjectId");
                b.ToTable("Tasks");
                b.HasData(
                    new { Id = 1, Title = "Design mockups", Description = "Create initial design mockups", Status = 2, ProjectId = 1, AssignedUserId = 1 },
                    new { Id = 2, Title = "Implement API", Description = "Build REST API endpoints", Status = 1, ProjectId = 1, AssignedUserId = 2 },
                    new { Id = 3, Title = "Setup CI/CD", Description = "Configure pipeline", Status = 0, ProjectId = 2 });
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.User", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd()
                    .HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                b.Property<string>("Email").IsRequired()
                    .HasMaxLength(150).HasColumnType("nvarchar(150)");
                b.Property<string>("FirstName").IsRequired()
                    .HasMaxLength(50).HasColumnType("nvarchar(50)");
                b.Property<string>("LastName").IsRequired()
                    .HasMaxLength(50).HasColumnType("nvarchar(50)");
                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.ToTable("Users");
                b.HasData(
                    new { Id = 1, FirstName = "Giorgi", LastName = "Beridze", Email = "giorgi.beridze@example.com" },
                    new { Id = 2, FirstName = "Nino", LastName = "Kapanadze", Email = "nino.kapanadze@example.com" });
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.Comment", b =>
            {
                b.HasOne("TaskManagement.Core.Entities.TaskItem", "Task")
                    .WithMany("Comments")
                    .HasForeignKey("TaskId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                b.Navigation("Task");
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.TaskItem", b =>
            {
                b.HasOne("TaskManagement.Core.Entities.User", "AssignedUser")
                    .WithMany("AssignedTasks")
                    .HasForeignKey("AssignedUserId")
                    .OnDelete(DeleteBehavior.SetNull);
                b.HasOne("TaskManagement.Core.Entities.Project", "Project")
                    .WithMany("Tasks")
                    .HasForeignKey("ProjectId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                b.Navigation("AssignedUser");
                b.Navigation("Project");
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.Project", b =>
            {
                b.Navigation("Tasks");
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.TaskItem", b =>
            {
                b.Navigation("Comments");
            });

            modelBuilder.Entity("TaskManagement.Core.Entities.User", b =>
            {
                b.Navigation("AssignedTasks");
            });
#pragma warning restore 612, 618
        }
    }
}
