using CloudPart3.Areas.Identity.Data;
using CloudPart3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeckDreams.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //checking if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.IsLoggedIn = false;
                return View(new List<Cart>()); //returning an empty list if the user is not logged in
            }

            ViewBag.IsLoggedIn = true;

            //fetching userId as a string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //retrieving cart items for the current logged in user
            var cartItems = await _context.Cart
                .Where(cart => cart.Id == userIdString)
                .Include(cart => cart.product)
                .ToListAsync();

            //calculating subtotal of all cart items
            double subtotal = cartItems.Sum(item => item.productPrice * item.Quantity);
            ViewBag.Subtotal = subtotal;

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            //checking if the user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Please log in to add items to your cart" });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //fetching product ID from azure SQL database
            var product = await _context.Product.FindAsync(productId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (userIdString == null)
            {
                return Json(new { success = false, message = "User ID is missing. Please log in again." });
            }

            //checking if the item is already in the cart
            var cartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.Id.ToString() == userIdString && c.productID == productId);

            if (cartItem == null) //if there are no cart items
            {
                cartItem = new Cart //create a new Cart table and populate it with the product the user added to their cart
                {
                    Id = userIdString,
                    productID = product.productID,
                    productName = product.productName,
                    productDescription = product.productDescription,
                    ImageURL = product.ImageURL,
                    productPrice = product.productPrice,
                    Quantity = 1
                };

                //adding cart item to the database
                _context.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += 1; //if there already is the same product in their cart, increase the quantity by 1
                _context.Update(cartItem); //update the quantity
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Please log in to update cart items." });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdString == null)
            {
                return Json(new { success = false, message = "User ID is missing. Please log in again." });
            }

            //fetching cart item
            var cartItem = await _context.Cart
                .Include(c => c.product)
                .FirstOrDefaultAsync(c => c.Id == userIdString && c.productID == productId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            //validating quantity (e.g non-negative)
            if (quantity < 1)
            {
                return Json(new { success = false, message = "Quantity must be at least 1." });
            }

            //updating quantity
            cartItem.Quantity = quantity;
            _context.Update(cartItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Quantity updated successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Please log in to remove items from your cart." });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdString == null)
            {
                return Json(new { success = false, message = "User ID is missing. Please log in again." });
            }

            //fetching cart item
            var cartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.Id == userIdString && c.productID == productId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            //remove item
            _context.Cart.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item removed from cart." });
        }

    }
}
