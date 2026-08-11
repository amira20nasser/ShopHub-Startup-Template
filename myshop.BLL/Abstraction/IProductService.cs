using myshop.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace myshop.BLL.Abstraction
{
    public interface IProductService
    {

        Task<IReadOnlyList<ProductDto>> GetWithCategory();
    }
}
