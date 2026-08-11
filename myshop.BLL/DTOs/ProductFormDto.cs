using System.ComponentModel.DataAnnotations;

namespace myshop.BLL.DTOs
{
    public class ProductFormDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public string? Img { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
