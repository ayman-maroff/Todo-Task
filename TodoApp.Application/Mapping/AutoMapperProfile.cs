using AutoMapper;
using TodoApp.Domain.Entities;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TodoItem, TodoDto>()
          .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Invitation, InvitationDto>().ReverseMap();
        }
    }
}