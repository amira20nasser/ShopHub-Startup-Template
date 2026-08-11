using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.BLL.Abstraction
{
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetAllNames();
    }
}
