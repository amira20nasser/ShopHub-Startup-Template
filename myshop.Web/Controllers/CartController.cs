using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Abstraction;

namespace myshop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult Add(int productId)
        {
            var product = _productService.GetById(productId).Result;
            if (product == null)
                return NotFound();

            _cartService.AddItem(
                product.Id,
                product.Name,
                product.Price,
                product.Img);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveItem(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int productId)
        {
            _cartService.IncreaseQuantity(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int productId)
        {
            _cartService.DecreaseQuantity(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}
