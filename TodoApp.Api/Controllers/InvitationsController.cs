using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Interfaces;
using TodoApp.Application.DTOs;
using AutoMapper;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using FluentValidation;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TodoApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public InvitationsController(IUserService userService,IInvitationService invitationService, IMapper mapper)
        {
            _invitationService = invitationService;
            _mapper = mapper;
            _userService = userService;

        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> SendInvitation([FromBody] CreateInvitationDto invitationDto, [FromServices] IValidator<CreateInvitationDto> validator)
        {
            var currentUserId = _userService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            var validationResult = await validator.ValidateAsync(invitationDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors
                    .Select(e => e.ErrorMessage));
            }
            var message = invitationDto.Message ?? "أنت مدعو للانضمام إلى TodoApp!";
            await _invitationService.SendInvitationAsync(invitationDto.Email, message, currentUserId.Value);
            return Ok(new { Message = "تم إرسال الدعوة!" });
        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetInvitations()
        {
            var invitations = await _invitationService.GetAllAsync();
            var invitationDtos = _mapper.Map<IEnumerable<InvitationDto>>(invitations);
            return Ok(invitationDtos);
        }
        [Authorize(Roles = "Owner")]
        [HttpDelete("{invitationId}")]
        public async Task<IActionResult> DeleteInvitation(Guid invitationId)
        {
            var currentUserId = _userService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }
                await _invitationService.DeleteAsync(invitationId, currentUserId.Value);
                return Ok(new { Message = "Deleted Successfully" }); 
        }

    }
}
