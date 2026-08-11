using AutoMapper;
using myshop.BLL.DTOs;
using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.BLL.Mappers
{
    public class ProductProfile : Profile
    {       
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>().ForMember(d =>d.CategoryName,s=>s.MapFrom(m=>m.Category.Name));
        }
    }
}
