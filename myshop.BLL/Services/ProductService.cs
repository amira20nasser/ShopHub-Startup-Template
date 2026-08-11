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
        private const string ImagesFolder = "Images/Products";

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

            productRepo.Remove(product);
            await unitOfWork.SaveChangesAsync();

            fileService.DeleteFile(product.Img);
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
    }
}
