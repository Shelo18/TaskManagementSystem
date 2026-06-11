using TaskManagement.Core.Enums;

namespace TaskManagement.Core.DTOs.Task;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? AssignedUserId { get; set; }
    public string? AssignedUserName { get; set; }
}

public class CreateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public int ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
}

public class UpdateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public int ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
}

public class TaskQueryParameters : TaskManagement.Core.DTOs.Common.QueryParameters
{
    public int? ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
    public TaskItemStatus? Status { get; set; }
}
