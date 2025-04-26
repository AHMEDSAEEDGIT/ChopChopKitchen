using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChopChopKitchen.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }

        [NotMapped] // not mapped to the database
        public IFormFile? ImageFile { get; set; }
        public string ImageUrl { get; set; } = "https://via.placeholder.com/150"; //default image placeholder

        [ValidateNever] //skip navigation property
        public Category? Category { get; set; } // products belongs to a category

        [ValidateNever]
        public ICollection<OrderItem>? OrderItems { get; set; } // a product can be in many order items
        
        [ValidateNever]
        public ICollection<ProductIngredient>? ProductIngredients { get; set; } // a product can have many ingredients
    }
}