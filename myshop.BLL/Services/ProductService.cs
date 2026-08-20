using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL;
using myshop.Entities.Models;

namespace myshop.BLL.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService) : IProductService
    {
        private const string ImagesFolder = "uploads/products";

        public async Task<ProductFormDto> Create(ProductFormDto productDto, IFormFile? file)
        {
            var uploadedPath = await fileService.UploadFileAsync(file, ImagesFolder);
            if (uploadedPath != null)
                productDto.Img = uploadedPath;

            var product = mapper.Map<Product>(productDto);

            var productRepo = unitOfWork.GetRepository<Product, int>();
            await productRepo.AddAsync(product);

            try
            {
                await unitOfWork.SaveChangesAsync();
            }
            catch
            {
                if (uploadedPath != null)
                    fileService.DeleteFile(uploadedPath);
                throw;
            }

            return mapper.Map<ProductFormDto>(product);
        }

        public async Task<bool> Edit(ProductFormDto productDto, IFormFile? file)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var product = await productRepo.GetById(productDto.Id);
            if (product == null)
                return false;

            if (file != null)
            {
                var uploadedPath = await fileService.UploadFileAsync(file, ImagesFolder);
                if (uploadedPath != null)
                {
                    fileService.DeleteFile(product.Img);
                    product.Img = uploadedPath;
                    productDto.Img = uploadedPath;
                }
            }

            mapper.Map(productDto, product);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var product = await productRepo.GetById(id);
            if (product == null)
                return false;

            fileService.DeleteFile(product.Img);

            productRepo.Remove(product);
            await unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ProductFormDto?> GetById(int id)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var product = await productRepo.GetById(id);
            return mapper.Map<ProductFormDto>(product);
        }

        public async Task<IReadOnlyList<ProductDto>> GetWithCategory()
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var products = await productRepo.GetAll(
                products => products.Include(product => product.Category));

            return mapper.Map<IReadOnlyList<ProductDto>>(products);
        }

        public async Task<PagedResultDto<ProductDto>> GetPagedAsync(ProductQueryDto query)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var queryable = productRepo.GetQueryable()
                .Include(p => p.Category);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.ToLower();
                queryable = queryable.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            queryable = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
            {
                ("name", "desc") => queryable.OrderByDescending(p => p.Name),
                ("name", "asc") => queryable.OrderBy(p => p.Name),
                ("price", "desc") => queryable.OrderByDescending(p => p.Price),
                ("price", "asc") => queryable.OrderBy(p => p.Price),
                _ => queryable.OrderBy(p => p.Name)
            };

            var totalItems = await queryable.CountAsync();

            var items = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Img = p.Img,
                    Price = p.Price,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return new PagedResultDto<ProductDto>
            {
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems
            };
        }
    }
}
