using CloudPart3.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace CloudPart3.Models
{
    public class Cart
    {
        [Key]
        public int cartID { get; set; }
        public string Id { get; set; } //this is to associate the cart with the user
        public ApplicationUser applicationUser { get; set; }
        public int productID { get; set; }
        public Product product { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public string? ImageURL { get; set; }
        public double productPrice { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
