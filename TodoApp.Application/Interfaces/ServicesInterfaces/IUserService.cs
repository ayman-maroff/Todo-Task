using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Core.Identity;

namespace TodoApp.Application.Interfaces.ServicesInterfaces
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllGuestsAsync(int page, int pageSize);
        bool IsEmailUnique(string email);
        Guid? GetCurrentUserId();

    }
}
