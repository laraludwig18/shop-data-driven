using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(60)]
        [MinLength(3)]
        public string Title { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}