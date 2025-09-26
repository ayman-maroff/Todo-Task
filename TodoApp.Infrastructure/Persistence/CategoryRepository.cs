using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces.RepoInterfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _ctx;

        public CategoryRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(Category category)
        {
            await _ctx.Categories.AddAsync(category);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _ctx.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _ctx.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }
    }
}
