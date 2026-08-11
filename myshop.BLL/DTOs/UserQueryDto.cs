namespace myshop.BLL.DTOs
{
    public class UserQueryDto
    {
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
