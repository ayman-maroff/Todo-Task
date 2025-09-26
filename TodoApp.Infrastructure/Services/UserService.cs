using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using TodoApp.Core.Identity;
using TodoApp.Infrastructure.Persistence;


namespace TodoApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserService(UserManager<ApplicationUser> userManager, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllGuestsAsync(int page, int pageSize)
        {
            var guests = await _userManager.GetUsersInRoleAsync("Guest");
            var pagedGuests = guests
                .Skip((page - 1) * pageSize) 
                .Take(pageSize);

            return pagedGuests;
        }
        public bool IsEmailUnique(string email)
        {
            return !_context.Users.Any(u => u.Email == email);
        }

        public Guid? GetCurrentUserId()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }

    }
}
