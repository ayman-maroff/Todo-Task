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
    public class CreateInvitationDtoValidator : AbstractValidator<CreateInvitationDto>
    {
        public CreateInvitationDtoValidator(IUserService userService)
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        
        }
    }
}
