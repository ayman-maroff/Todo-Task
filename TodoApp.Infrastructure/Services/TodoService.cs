using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces.RepoInterfaces;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using TodoApp.Domain.Entities;
using TodoApp.Core.Identity;


namespace TodoApp.Infrastructure.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoService(ITodoRepository todoRepository, UserManager<ApplicationUser> userManager)
        {
            _todoRepository = todoRepository;
            _userManager = userManager;

        }

        public async Task<TodoItem> CreateTodoAsync(CreateTodoDto createTodoDto, Guid createdByUserId)
        {
            var todoItem = new TodoItem
            {
                Title = createTodoDto.Title,
                Description = createTodoDto.Description,
                DueDate = createTodoDto.DueDate,
                Priority = createTodoDto.Priority,
                CreatedByUserId = createdByUserId,
                CategoryId = createTodoDto.CategoryId
            };

            await _todoRepository.AddAsync(todoItem);
            await _todoRepository.SaveChangesAsync();
            return todoItem;
        }
        public async Task UpdateTodoAsync(Guid id, UpdateTodoDto updateTodoDto)
        {
            var todoItem = await _todoRepository.GetByIdAsync(id);
            if (todoItem != null)
            {
                todoItem.Title = updateTodoDto.Title ?? todoItem.Title;
                todoItem.Description = updateTodoDto.Description ?? todoItem.Description;
                todoItem.DueDate = updateTodoDto.DueDate ?? todoItem.DueDate;
                todoItem.Priority = updateTodoDto.Priority ?? todoItem.Priority;
                todoItem.CategoryId = updateTodoDto.CategoryId ?? todoItem.CategoryId;

                _todoRepository.Update(todoItem);
                await _todoRepository.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<TodoItem>> GetTodosWithFiltersAsync(
      Guid? categoryId, Priority? priority, int pageNumber, int pageSize)
        {
            var todos = await _todoRepository.GetAllAsync();

            var query = todos.AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }
            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }


        public async Task<TodoItem?> GetTodoByIdAsync(Guid id)
        {
            return await _todoRepository.GetByIdAsync(id);
        }

        public async Task MarkAsCompletedAsync(Guid id, Guid currentUser)
        {
            await _todoRepository.MarkAsCompletedAsync(id, currentUser);
        }

        public async Task DeleteTodoAsync(Guid id)
        {
           
              await  _todoRepository.Remove(id);
           
        }

        public async Task<IEnumerable<TodoItem>> GetTodosByAssignedUserAsync(Guid userId, Guid? categoryId, Priority? priority, int pageNumber, int pageSize)
        {
            var todos = await _todoRepository.GetByAssignedUserAsync(userId);

            var query = todos.AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }
            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public async Task<bool> AssignTodoToUserAsync(Guid todoId, Guid guestId)
        {
            var todo = await _todoRepository.GetByIdAsync(todoId);
            if (todo == null) return false;

            var guest = await _userManager.FindByIdAsync(guestId.ToString());
            if (guest == null)
            {
                throw new InvalidOperationException("The specified User does not exist.");
            }
            if (todo.AssignedToUserId != null)
            {
                throw new InvalidOperationException("This task is  assigned to Another person.");
            }
            todo.AssignedToUserId = guestId;
            _todoRepository.Update(todo);
            await _todoRepository.SaveChangesAsync();

            return true;
        }


    }

}
