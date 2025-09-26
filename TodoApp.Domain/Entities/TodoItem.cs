using TodoApp.Domain.Entities;
using TodoApp.Core.Identity;
public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid CreatedByUserId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public ApplicationUser? AssignedToUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
