using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL;
using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.BLL.Services
{
    public class ProductService(IUnitOfWork unitOfWork,IMapper mapper) : IProductService
    {
        public async Task<IReadOnlyList<ProductDto>> GetWithCategory()
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var  categoryInclude = (DbSet<Product> products) => products.Include(product => product.Category);

            var products  = await productRepo.GetAll((products) => products.Include(product => product.Category)) ;

            if (products == null)
            {
                return [];
            }            
            return mapper.Map<IReadOnlyList<ProductDto>>(products);
        }
    }
}
