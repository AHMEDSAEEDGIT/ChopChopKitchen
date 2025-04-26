using ChopChopKitchen.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChopChopKitchen.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepository;

        public ProductController(IRepository<Product> productRepository)
        {
            this._productRepository = productRepository;
        }
        public async  Task<IActionResult> Index()
        {
            return  View(await _productRepository.GetAllAsync());
        }
    }
}
