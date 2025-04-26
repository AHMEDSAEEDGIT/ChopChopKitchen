using ChopChopKitchen.Data;
using ChopChopKitchen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ChopChopKitchen.Controllers
{
    public class IngredientController : Controller
    {
        private readonly IRepository<Ingredient> _ingredientsRepo;
        public IngredientController(IRepository<Ingredient> ingredientRepo)
        {
            _ingredientsRepo = ingredientRepo;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _ingredientsRepo.GetAllAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await _ingredientsRepo.GetByIdAsync(id, new QueryOptions<Ingredient>() { Incldues = "ProductIngredients.Product" }));
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ingredient , Name")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                await _ingredientsRepo.AddAsync(ingredient);
                return RedirectToAction("Index");
            }
            return View(ingredient);
        }

        //Ingredient/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View(await _ingredientsRepo.GetByIdAsync(id, new QueryOptions<Ingredient> { Incldues = "ProductIngredients.Product" }));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Ingredient ingredient)
        {
            await _ingredientsRepo.DeleteAsync(ingredient.IngredientId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _ingredientsRepo.GetByIdAsync(id, new QueryOptions<Ingredient> { Incldues = "ProductIngredients.Product" }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                await _ingredientsRepo.UpdateAsync(ingredient);
                return RedirectToAction("Index");
            }
            return View(ingredient);
        }

    }
}
