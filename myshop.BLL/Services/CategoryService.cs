using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.BLL.Abstraction;
using myshop.DAL;
using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.BLL.Services
{
    public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
    {
        public async Task<IEnumerable<SelectListItem>> GetAllNames()
        {
            var categoryRepo = unitOfWork.GetRepository<Category, int>();
            var categories = await categoryRepo.GetAll(); 

            return categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }
    }
}
