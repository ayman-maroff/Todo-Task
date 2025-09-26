using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.RepoInterfaces
{
    public interface ITodoRepository
    {
        Task<TodoItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task AddAsync(TodoItem item);
        void Update(TodoItem item);
        Task Remove(Guid id);
        Task SaveChangesAsync();
        Task MarkAsCompletedAsync(Guid id, Guid currentUser);
        Task<IEnumerable<TodoItem>> GetByAssignedUserAsync(Guid userId);
    }
}
