using Microsoft.AspNetCore.Http;
using myshop.BLL.DTOs;

namespace myshop.BLL.Abstraction
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductDto>> GetWithCategory();
        Task<PagedResultDto<ProductDto>> GetPagedAsync(ProductQueryDto query);

        Task<ProductFormDto?> GetById(int id);

        Task<ProductFormDto> Create(ProductFormDto productDto, IFormFile? file);

        Task<bool> Edit(ProductFormDto productDto, IFormFile? file);

        Task<bool> Delete(int id);
    }
}
