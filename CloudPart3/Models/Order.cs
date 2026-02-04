using CloudPart3.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CloudPart3.Models
{
    public class Order
    {
        [Key]
        public int orderID { get; set; }

        [BindNever] //to make sure the user id does not bind to the order creation form
        public string? Id { get; set; }

        public ApplicationUser? applicationUser { get; set; }

        public int productID { get; set; }

        public string orderStatus { get; set; } = "Pending"; //default state of order is pending

        [Required(ErrorMessage = "Delivery address is required.")]
        [StringLength(200, ErrorMessage = "Address should not exceed 200 characters.")]
        public string orderAddress { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public string paymentMethod { get; set; }

        [Required]
        public DateTime orderDate { get; set; }
    }
}
