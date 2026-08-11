using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.BLL.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string CategoryName { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
