using TaskManagement.Core.Enums;

namespace TaskManagement.Core.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
