using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.ServicesInterfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(string name);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid id);

        bool IsNameUnique(string name);

    }
}
