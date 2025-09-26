
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using TodoApp.Core.Identity;
namespace TodoApp.Api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "Owner")]
        [HttpGet("guests")]
        public async Task<IActionResult> GetAllGuests(int page = 1, int pageSize = 10)
        {
            var guests = await _userService.GetAllGuestsAsync(page, pageSize);

            var result = guests.Select(g => new
            {
                g.Id,
                g.UserName,
                g.Email
            });

            return Ok(result);
        }
    }

}
