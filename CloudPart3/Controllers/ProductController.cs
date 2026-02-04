using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudPart3.Areas.Identity.Data;
using CloudPart3.Models;
using CloudPart3.Services;

namespace CloudPart3.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly BlobStorageService _blobStorageService; 

        public ProductController(ApplicationDbContext context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Product.ToListAsync(); //fetching all the products from Azure SQL database

            if (products == null || !products.Any())
            {
                Console.WriteLine("No products found in the database!");
            }

            return View(products);
        }


        // GET: Product/Create
        public IActionResult Create()
        {
            // Check if the user is an admin
            if (User.IsInRole("Admin"))
            {
                return View();
            }
            return Unauthorized(); //redirect to another page if they are not an admin
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0) //uploading file before checking if modelstate is valid to avoid errors
            {
                using var stream = imageFile.OpenReadStream();
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var imageUrl = await _blobStorageService.UploadFileAsync(stream, fileName, "product-images"); //uploading the image to product-images container
                product.ImageURL = imageUrl;
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Product image is required");
                return View(product);
            }

            if (ModelState.IsValid)
            {
                _context.Product.Add(product); //adds product to Azure SQL database 
                await _context.SaveChangesAsync(); //saves details

                return RedirectToAction("Index", "Product");
            }

            return View(product);
        }

        // GET: Product/Edit/5 -- this is for admins to edit product details
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Product.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            } else
            {
                return Unauthorized();
            }
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product updatedProduct, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Product.FindAsync(id); //fetching product id 

                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (imageFile != null && imageFile.Length > 0) //if imagefile is not null
                {
                    //delete old image
                    await _blobStorageService.DeleteFileAsync(existingProduct.ImageURL, "product-images");

                    //upload new image
                    using var stream = imageFile.OpenReadStream();
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    var imageUrl = await _blobStorageService.UploadFileAsync(stream, fileName, "product-images");
                    existingProduct.ImageURL = imageUrl;
                }

                //update other fields
                existingProduct.productName = updatedProduct.productName;
                existingProduct.productDescription = updatedProduct.productDescription;
                existingProduct.productPrice = updatedProduct.productPrice;

                _context.Product.Update(existingProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Product");
            }
            return View(updatedProduct);
        }

        // GET: Product/Delete/5 -- this is for admins to delete a product 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.productID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int productID)
        {
            var product = await _context.Product.FindAsync(productID);
            if (product == null)
            {
                return NotFound();
            }

            //delete image from Blob Storage
            await _blobStorageService.DeleteFileAsync(product.ImageURL, "product-images");

            //delete product from Table Storage
            _context.Product.Remove(product);

            return RedirectToAction("Index", "Product");
        }

        //public async Task<List<Product>> GetRandomProductsAsync(int count)
        //{
        //    var allProducts = _context.Product.ToListAsync();

        //    //get random products (ensure to handle the case where there are fewer than 3 products)
        //    var randomProducts = allProducts.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

        //    return randomProducts;
        //}

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.productID == id);
        }
    }
}
