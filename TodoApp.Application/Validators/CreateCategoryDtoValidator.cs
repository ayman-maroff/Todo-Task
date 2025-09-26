using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces.ServicesInterfaces;

namespace TodoApp.Application.Validators
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator(ICategoryService categoryService)
        {

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                 .Must(Name => categoryService.IsNameUnique(Name))
                .WithMessage("Name is already in use");

      
        }
    }
}
