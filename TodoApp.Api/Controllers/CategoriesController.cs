using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Interfaces;
using TodoApp.Application.DTOs;
using AutoMapper;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using FluentValidation;

namespace TodoApp.Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto, [FromServices] IValidator<CreateCategoryDto> validator)
        {
            var validationResult = await validator.ValidateAsync(createCategoryDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors
                    .Select(e => e.ErrorMessage));
            }
            var category = await _categoryService.CreateCategoryAsync(createCategoryDto.Name);
            var categoryDtoMapped = _mapper.Map<CategoryDto>(category);
            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, categoryDtoMapped);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }
    }
}
