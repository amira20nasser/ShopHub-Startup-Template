using AutoMapper;
using myshop.BLL.DTOs;
using myshop.Entities.Models;

namespace myshop.BLL.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore());
        }
    }
}
