using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL;
using myshop.Entities.Models;

namespace myshop.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "AllCategories";
        private static readonly MemoryCacheEntryOptions CacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAll()
        {
            if (_cache.TryGetValue(CacheKey, out IReadOnlyList<CategoryDto>? cached) && cached != null)
                return cached;

            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            var categories = await categoryRepo.GetAll();
            var result = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);

            _cache.Set(CacheKey, result, CacheOptions);

            return result;
        }

        public async Task<CategoryDto?> GetById(int id)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            var category = await categoryRepo.GetById(id);
            return _mapper.Map<CategoryDto?>(category);
        }

        public async Task<CategoryDto> Create(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.CreatedTime = DateTime.Now;

            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            await categoryRepo.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(CacheKey);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> Update(CategoryDto categoryDto)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, int>();

            var category = await categoryRepo.GetById(categoryDto.Id);
            if (category == null)
            {
                return false;
            }

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(CacheKey);

            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, int>();

            var category = await categoryRepo.GetById(id);
            if (category == null)
            {
                return false;
            }

            categoryRepo.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove(CacheKey);

            return true;
        }
    }
}
