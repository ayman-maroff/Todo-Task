using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Domain.Entities
{
    public class Invitation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = null!;
        public string? Message { get; set; }
        public Guid InvitedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
