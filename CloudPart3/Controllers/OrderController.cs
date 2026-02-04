using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudPart3.Areas.Identity.Data;
using CloudPart3.Models;
using System.Security.Claims;

namespace CloudPart3.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            //fetching currrently logged in user
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if userid null redirect to login page
            if (userIdString == null)
            {
                return RedirectToAction("Login", "User");
            }

            //fetch orders for user that is logged in
            // var orders = await _tableStorageService.GetOrdersByUserIdAsync(userId);

            //retrieving orders for the current logged in user
            var orders = await _context.Order
                .Where(order => order.Id == userIdString)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Cart()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //getting cart items of currently logged in user
            var cartItems = await _context.Cart
                .Where(cart => cart.Id == userIdString)
                .Include(cart => cart.product)
                .ToListAsync();

            return View(cartItems);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {

            //fetch user id before checking model state to prevent incorrect binding in form (order/create) submission 
            //getting current logged in users id
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //assigning the user id in order model to the user id of the current logged in user
            order.Id = userIdString;

            //if modelstate is not valid redirect to home
            if (!ModelState.IsValid)
            {
                //log the errors for debugging
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                //return the view with the current model to display validation errors
                return View(order);
            }

            //check if order.Id is valid
            if (order.Id == null)
            {
                ModelState.AddModelError("", "Invalid user ID");
                return View(order);
            }

            //retrieving cart items for the current logged in user
            var cartItems = await _context.Cart
                .Where(cart => cart.Id == order.Id)
                .Include(cart => cart.product)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty or unable to retrieve cart items.");
                return RedirectToAction("Index", "Cart");
            }

            foreach (var cartItem in cartItems)
            {
                //create a new order for each item in the cart
                var newOrder = new Order
                {
                    Id = order.Id,
                    productID = cartItem.productID,
                    orderStatus = "Pending",
                    orderAddress = order.orderAddress,
                    paymentMethod = order.paymentMethod,
                    orderDate = DateTime.UtcNow
                };

                //adding order to the ordersTable
                 _context.Add(newOrder);
            }

            //clear user's cart after placing order
            foreach (var cartItem in cartItems)
            {
                  _context.Remove(cartItem);
            }

            await _context.SaveChangesAsync();

            //redirect to confirmation page
            return RedirectToAction("Confirmation", "Order");
        }

        //confirmation of order view
        public IActionResult Confirmation()
        {
            return View();
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("orderID,Id,productID,orderStatus,orderAddress,paymentMethod")] Order order)
        {
            if (id != order.orderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.orderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.orderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.orderID == id);
        }
    }
}
