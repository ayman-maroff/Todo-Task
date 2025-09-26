using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.Interfaces.RepoInterfaces;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly AppDbContext _context;

        public CategoryService(ICategoryRepository categoryRepository, AppDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context= context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category> CreateCategoryAsync(string name)
        {
            var category = new Category { Name = name };
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }
        public bool IsNameUnique(string name)
        {
            return !_context.Categories.Any(u => u.Name == name);
        }
    }
}
