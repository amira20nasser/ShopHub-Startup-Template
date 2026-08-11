using AutoMapper;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL;
using myshop.Entities.Models;

namespace myshop.BLL.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        public async Task<IReadOnlyList<CategoryDto>> GetAll()
        {
            var categoryRepo = unitOfWork.GetRepository<Category, int>();
            var categories = await categoryRepo.GetAll();
            return mapper.Map<IReadOnlyList<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetById(int id)
        {
            var categoryRepo = unitOfWork.GetRepository<Category, int>();
            var category = await categoryRepo.GetById(id);
            return mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> Create(CategoryDto categoryDto)
        {
            var category = mapper.Map<Category>(categoryDto);
            category.CreatedTime = DateTime.Now;

            var categoryRepo = unitOfWork.GetRepository<Category, int>();
            await categoryRepo.AddAsync(category);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> Update(CategoryDto categoryDto)
        {
            var categoryRepo = unitOfWork.GetRepository<Category, int>();

            var category = await categoryRepo.GetById(categoryDto.Id);
            if (category == null)
            {
                return false;
            }

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var categoryRepo = unitOfWork.GetRepository<Category, int>();

            var category = await categoryRepo.GetById(id);
            if (category == null)
            {
                return false;
            }

            categoryRepo.Remove(category);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
