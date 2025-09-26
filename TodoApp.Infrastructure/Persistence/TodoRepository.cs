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
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _ctx;
        public TodoRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(TodoItem item)
        {
            if (item.CategoryId.HasValue)
            {
                var categoryExists = await _ctx.Categories
                    .AnyAsync(c => c.Id == item.CategoryId.Value); 

                if (!categoryExists)
                {
                    throw new InvalidOperationException("The specified category does not exist.");
                }
            }

            await _ctx.TodoItems.AddAsync(item);
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync() => await _ctx.TodoItems.Include(t => t.Category).ToListAsync();

        public async Task<TodoItem?> GetByIdAsync(Guid id) => await _ctx.TodoItems.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
        
        public async Task<IEnumerable<TodoItem>> GetByAssignedUserAsync(Guid userId)
            => await _ctx.TodoItems.Where(t => t.AssignedToUserId == userId).ToListAsync();

        public async Task Remove(Guid id)
        {
            var todoItem = await _ctx.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
            if (todoItem != null)
            {
                _ctx.TodoItems.Remove(todoItem);
                await SaveChangesAsync();
            }
        }
        public void Update(TodoItem item)
        {
            if (item.CategoryId.HasValue)
            {
                var categoryExists = _ctx.Categories
                    .Any(c => c.Id == item.CategoryId.Value);

                if (!categoryExists)
                {
                    throw new InvalidOperationException("The specified category does not exist.");
                }
            }
            _ctx.TodoItems.Update(item);
        }
        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();

        public async Task MarkAsCompletedAsync(Guid id, Guid currentUser)
        {
            var todoItem = await _ctx.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todoItem == null)
            {
                throw new InvalidOperationException("Task not found");
            }
            if (todoItem.AssignedToUserId != currentUser)
            {
                throw new InvalidOperationException("This task is not assigned to you.");
            }
            todoItem.IsCompleted = true;
            _ctx.TodoItems.Update(todoItem);

            await SaveChangesAsync();
        }

    }
}
