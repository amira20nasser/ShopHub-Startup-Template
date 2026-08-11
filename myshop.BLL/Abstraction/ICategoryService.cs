using myshop.BLL.DTOs;

namespace myshop.BLL.Abstraction
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAll();

        Task<CategoryDto?> GetById(int id);

        Task<CategoryDto> Create(CategoryDto categoryDto);

        Task<bool> Update(CategoryDto categoryDto);

        Task<bool> Delete(int id);
    }
}
