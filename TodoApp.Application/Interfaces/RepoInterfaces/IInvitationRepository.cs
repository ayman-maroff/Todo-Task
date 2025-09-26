using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.RepoInterfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation?> GetByIdAsync(Guid id);
        Task<IEnumerable<Invitation>> GetAllAsync();
        Task AddAsync(Invitation invitation);
        Task SaveChangesAsync();
        Task DeleteAsync(Guid id, Guid requestedByUserId);
    }

}
