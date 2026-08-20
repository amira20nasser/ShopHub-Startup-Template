using myshop.BLL.DTOs;

namespace myshop.BLL.Abstraction
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddItem(int productId, string productName, decimal price, string? imageUrl, int quantity = 1);
        void RemoveItem(int productId);
        void IncreaseQuantity(int productId);
        void DecreaseQuantity(int productId);
        void ClearCart();
    }
}
