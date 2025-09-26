using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.DTOs
{
    public class InvitationDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
