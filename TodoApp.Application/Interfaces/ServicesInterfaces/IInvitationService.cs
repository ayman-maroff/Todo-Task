using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.ServicesInterfaces
{
    public interface IInvitationService
    {
        Task SendInvitationAsync(string email, string message, Guid invitedByUserId);
        Task<IEnumerable<Invitation>> GetAllAsync();
        Task DeleteAsync(Guid invitationId, Guid requestedByUserId);
    }
}
