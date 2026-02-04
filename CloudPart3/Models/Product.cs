using System.ComponentModel.DataAnnotations;

namespace CloudPart3.Models
{
    public class Product
    {
        [Key] 
        public int productID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string productName { get; set; }

        public string productDescription { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than zero")]
        public double productPrice { get; set; }

        public string? ImageURL { get; set; }
    }
}
