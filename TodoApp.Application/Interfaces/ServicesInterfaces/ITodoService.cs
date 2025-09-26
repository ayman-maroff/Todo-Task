using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.ServicesInterfaces
{
    public interface ITodoService
    {
        Task<TodoItem> CreateTodoAsync(CreateTodoDto createTodoDto, Guid createdByUserId);
        Task<IEnumerable<TodoItem>> GetTodosWithFiltersAsync(
      Guid? categoryId, Priority? priority, int pageNumber, int pageSize);
        Task UpdateTodoAsync(Guid id, UpdateTodoDto updateTodoDto);
        Task<TodoItem?> GetTodoByIdAsync(Guid id);
        Task MarkAsCompletedAsync(Guid id, Guid currentUser);
        Task DeleteTodoAsync(Guid id);
        Task<IEnumerable<TodoItem>> GetTodosByAssignedUserAsync(Guid userId, Guid? categoryId, Priority? priority, int pageNumber, int pageSize);
        Task<bool> AssignTodoToUserAsync(Guid todoId, Guid guestId);
    }
}
