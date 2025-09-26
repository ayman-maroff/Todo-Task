using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using TodoApp.Infrastructure.Services;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces.RepoInterfaces;
using TodoApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TodoApp.Core.Identity;
using FluentAssertions;
using System.Collections.Generic;

namespace TodoApp.Tests.Unit
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _mockRepo = new Mock<ITodoRepository>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
            );
            _service = new TodoService(_mockRepo.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task CreateTodoAsync_ShouldAddTodo()
        {
           
            var dto = new CreateTodoDto
            {
                Title = "Test Task",
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = Priority.High,
                CategoryId = Guid.NewGuid()
            };
            var userId = Guid.NewGuid();

           
            var result = await _service.CreateTodoAsync(dto, userId);

       
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Task");
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_ShouldUpdateExistingTodo()
        {
      
            var id = Guid.NewGuid();
            var existingTodo = new TodoItem { Id = id, Title = "Old" };
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingTodo);

            var dto = new UpdateTodoDto { Title = "Updated Title" };

            await _service.UpdateTodoAsync(id, dto);

            existingTodo.Title.Should().Be("Updated Title");
            _mockRepo.Verify(r => r.Update(existingTodo), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AssignTodoToUserAsync_ShouldThrow_WhenUserNotFound()
        {
            var todoId = Guid.NewGuid();
            var guestId = Guid.NewGuid();
            var todo = new TodoItem { Id = todoId };

            _mockRepo.Setup(r => r.GetByIdAsync(todoId)).ReturnsAsync(todo);
            _mockUserManager.Setup(m => m.FindByIdAsync(guestId.ToString())).ReturnsAsync((ApplicationUser)null);

            Func<Task> act = async () => await _service.AssignTodoToUserAsync(todoId, guestId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The specified User does not exist.");
        }
    }
}
