using FluentValidation;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using System;

namespace TodoApp.Application.Validators
{
    public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
    {
        public UpdateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.")
                .When(x => x.Title != null);

            RuleFor(x => x.Priority)
                   .IsInEnum().WithMessage("Invalid Priority value. The correct values are (Low, Medium, High).")
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Now).When(x => x.DueDate.HasValue)
                .WithMessage("DueDate must be in the future.")
                .When(x => x.DueDate.HasValue);
            RuleFor(x => x.CategoryId)
                .NotEmpty().When(x => x.CategoryId.HasValue)
                .WithMessage("CategoryId must be a valid GUID.");
        }
    }
}
