using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.DTOs
{
    public class TodoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }
}
