using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.DTOs
{
    public class CreateInvitationDto
    {
        public string Email { get; set; } = null!;
        public string? Message { get; set; }
    }
}
