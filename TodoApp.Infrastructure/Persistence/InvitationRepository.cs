using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces.RepoInterfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly AppDbContext _ctx;

        public InvitationRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Invitation?> GetByIdAsync(Guid id)
        {
            return await _ctx.Invitations.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Invitation>> GetAllAsync()
        {
            return await _ctx.Invitations.ToListAsync();
        }

        public async Task AddAsync(Invitation invitation)
        {
            await _ctx.Invitations.AddAsync(invitation);
        }

        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid invitationId, Guid requestedByUserId)
        {
            var invitation = await _ctx.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId);

            if (invitation == null)
            {
                throw new InvalidOperationException("Invitation not found.");
            }

            if (invitation.InvitedByUserId != requestedByUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this invitation.");
            }

            _ctx.Invitations.Remove(invitation);
            await _ctx.SaveChangesAsync();
        }

    }
}
