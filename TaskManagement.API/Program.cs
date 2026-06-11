using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TaskManagement.API.Middleware;
using TaskManagement.Core.DTOs.Comment;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Validators;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Mappings;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── AutoMapper ────────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ── Repositories ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// ── FluentValidation ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();
builder.Services.AddScoped<IValidator<UpdateUserDto>, UpdateUserValidator>();
builder.Services.AddScoped<IValidator<CreateProjectDto>, CreateProjectValidator>();
builder.Services.AddScoped<IValidator<UpdateProjectDto>, UpdateProjectValidator>();
builder.Services.AddScoped<IValidator<CreateTaskItemDto>, CreateTaskItemValidator>();
builder.Services.AddScoped<IValidator<UpdateTaskItemDto>, UpdateTaskItemValidator>();
builder.Services.AddScoped<IValidator<CreateCommentDto>, CreateCommentValidator>();
builder.Services.AddScoped<IValidator<UpdateCommentDto>, UpdateCommentValidator>();

// ── Controllers & Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "A RESTful API for managing projects, tasks, users and comments."
    });
});

// ── CORS (useful for frontend or Supabase integrations) ───────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ── Auto-apply migrations on startup ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

// Needed for integration tests
public partial class Program { }
