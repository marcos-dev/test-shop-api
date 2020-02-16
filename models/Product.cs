
using System.ComponentModel.DataAnnotations;

namespace shop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Title { get; set; }

        [MaxLength(1024, ErrorMessage = "Este campo é limitado a 1024 caracteres")]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}