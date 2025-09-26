using FluentValidation;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using System;

namespace TodoApp.Application.Validators
{
    public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
    {
        public CreateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid Priority value. The correct values are (Low, Medium, High).");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("DueDate is required.")
                .GreaterThanOrEqualTo(DateTime.Now).WithMessage("DueDate must be in the future.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required.")
                .Must(x => Guid.TryParse(x.ToString(), out _)).WithMessage("CategoryId must be a valid GUID.");
        }
    }
}
