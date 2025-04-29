using ChopChopKitchen.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ChopChopKitchen.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Ingredient> _ingredientRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IRepository<Product> productRepository,
                                 IRepository<Ingredient> ingredientRepository, 
                                 IRepository<Category> categoryRepository,
                                 IWebHostEnvironment webHostEnvironment)
        {
            this._productRepository = productRepository;
            this._ingredientRepository = ingredientRepository;
            this._categoryRepository = categoryRepository;
            this._webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _productRepository.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Ingredients= await _ingredientRepository.GetAllAsync();
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            if (id == 0)
            {
                ViewBag.Operation = "Add";
                return View(new Product());
            }
            else
            {

                Product product = await _productRepository.GetByIdAsync(id, new QueryOptions<Product>
                {
                    Incldues = "ProductIngredients.Ingredient, Category"  
                });



                ViewBag.Operation = "Edit";
                return View(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Product product , int[] ingredientIds , int catId)
        {
            ViewBag.Ingredients = await _ingredientRepository.GetAllAsync();
            ViewBag.Categories = await _categoryRepository.GetAllAsync();

            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    //get wwwroot path appended to images directory
                    string uploadFolders = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    
                    //generate unique name for the file in case the user upload the same image again
                    string uniqueFileName = Guid.NewGuid().ToString()+"_" +product.ImageFile.FileName;
                    string filePath = Path.Combine(uploadFolders, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = uniqueFileName;
                } 

                if(product.ProductId == 0)
                {
                    product.CategoryId = catId;
                    

                    //add ingredients
                    foreach(int id in ingredientIds)
                    {
                        product.ProductIngredients?.Add(new ProductIngredient { IngredientId = id, ProductId = product.ProductId });
                    }

                    await _productRepository.AddAsync(product);
                    return RedirectToAction("Index","Product");
                }
                else
                {
                    var existingProduct = await _productRepository.GetByIdAsync(product.ProductId,new QueryOptions<Product> { Incldues="ProductIngredients"});
                    if (existingProduct == null)
                    {
                        ViewBag.Ingredients = await _ingredientRepository.GetAllAsync();
                        ViewBag.Categories = await _categoryRepository.GetAllAsync();

                        ModelState.AddModelError("", "Product not found.");
                        return View(product);
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price  = product.Price;
                    existingProduct.Stock  = product.Stock;
                    existingProduct.CategoryId  = product.CategoryId;

                    existingProduct?.ProductIngredients?.Clear();
                    foreach(int id in ingredientIds)
                    {
                        existingProduct?.ProductIngredients?.Add(new ProductIngredient { ProductId = product.ProductId, IngredientId = id });
                    }

                    try
                    {
                        await _productRepository.UpdateAsync(existingProduct);
                    }catch(Exception ex)
                    {
                        ModelState.AddModelError("", $"Error:{ex.GetBaseException().Message}");
                        ViewBag.Ingredients = await _ingredientRepository.GetAllAsync();
                        ViewBag.Categories = await _categoryRepository.GetAllAsync();

                    }
                }
            }
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Product not found.");
                return RedirectToAction("Index");
            }
        }
    }
}
