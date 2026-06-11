namespace TaskManagement.Core.DTOs.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TaskId { get; set; }
}

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public int TaskId { get; set; }
}

public class UpdateCommentDto
{
    public string Content { get; set; } = string.Empty;
}
