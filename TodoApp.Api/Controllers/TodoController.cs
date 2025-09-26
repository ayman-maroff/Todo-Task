using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Interfaces;
using TodoApp.Application.DTOs;
using AutoMapper;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public TodoController(IUserService userService,ITodoService todoService, IMapper mapper)
        {
            _todoService = todoService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetAllTodos(
         [FromQuery] Guid? categoryId = null,
         [FromQuery] Priority? priority = null,
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10)
        {
            var todos = await _todoService.GetTodosWithFiltersAsync(categoryId, priority, pageNumber, pageSize);
            var todoDtos = _mapper.Map<IEnumerable<TodoDto>>(todos);
            return Ok(todoDtos);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDto createTodoDto, [FromServices] IValidator<CreateTodoDto> validator)
        {
            var validationResult = await validator.ValidateAsync(createTodoDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors
                    .Select(e => e.ErrorMessage));
            }
            var currentUserId = _userService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            } 
                var todoItem = await _todoService.CreateTodoAsync(createTodoDto, currentUserId.Value);
                var createdTodoDto = _mapper.Map<TodoDto>(todoItem);
                return Ok(new { Message = "Task created successfully" });
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoDto updateTodoDto, [FromServices] IValidator<UpdateTodoDto> validator)
        {
            var validationResult = await validator.ValidateAsync(updateTodoDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors
                    .Select(e => e.ErrorMessage));
            }
                var existingTodo = await _todoService.GetTodoByIdAsync(id);
                if (existingTodo == null) return NotFound();
                await _todoService.UpdateTodoAsync(id, updateTodoDto);
                var updatedTodo = await _todoService.GetTodoByIdAsync(id);
                var updatedTodoDto = _mapper.Map<TodoDto>(updatedTodo);
                return Ok(updatedTodoDto); 
                  }


        [HttpGet("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetTodoByIdAsync(Guid id)
        {
            var toDo = await _todoService.GetTodoByIdAsync(id);
            if (toDo == null)
            {
                return NotFound(new { Message = "Task not found" });
            }
            var toDoDto = _mapper.Map<TodoDto>(toDo);
            return Ok(toDoDto);
        }


        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            var currentUserId = _userService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            var toDo = await _todoService.GetTodoByIdAsync(id);

            if (toDo == null)
            {
                return NotFound(new { Message = "Task not found" });
            }
                await _todoService.MarkAsCompletedAsync(id, currentUserId.Value);
                return Ok(new { Message = "Task marked as completed" });
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            var toDo = await _todoService.GetTodoByIdAsync(id);
            if (toDo == null)
            {
                return NotFound(new { Message = "Task not found" });
            }
            await _todoService.DeleteTodoAsync(id);
            return Ok(new { Message = "Task deleted successfully" });
        }


        [Authorize(Roles = "Guest")]
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks(
         [FromQuery] Guid? categoryId = null,
         [FromQuery] Priority? priority = null,
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10)
        {
            var currentUserId = _userService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            var todos = await _todoService.GetTodosByAssignedUserAsync(currentUserId.Value, categoryId, priority, pageNumber, pageSize);
            return Ok(todos);
        }


        [Authorize(Roles = "Owner")]
        [HttpPost("{todoId}/assign/{guestId}")]
        public async Task<IActionResult> AssignTask(Guid todoId, Guid guestId)
        {
                var success = await _todoService.AssignTodoToUserAsync(todoId, guestId);
                if (!success) return NotFound(new { Message = "Todo or Guest not found" });

                return Ok(new { Message = "Task assigned successfully" });
            
        }
    }
}
