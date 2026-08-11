
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL.Models;
using myshop.Entities.ViewModels;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;


        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var products = await _productService.GetWithCategory();

            return Json(new { data = products });
        }

        [HttpGet]
        public async  Task<IActionResult> Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new ProductFormDto(),
                CategoryList = await GetCategoryList()
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
               await _productService.Create(productVM.Product,file);

                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            productVM.CategoryList = await GetCategoryList();
            return View(productVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = await _productService.GetById(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            ProductVM productVM = new ProductVM()
            {
                Product = product,
                CategoryList = await GetCategoryList()
            };

            return View(productVM);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var updated = await _productService.Edit(productVM.Product, file);
                if (!updated)
                {
                    return NotFound();
                }

                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }

            productVM.CategoryList = await GetCategoryList();
            return View(productVM);
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var deleted = await _productService.Delete(id.Value);

            if (!deleted)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            return Json(new { success = true, message = "file has been Deleted" });
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoryList()
        {
            var categories = await _categoryService.GetAll();
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }
    }
}
