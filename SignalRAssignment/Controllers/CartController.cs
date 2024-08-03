using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalRAssignment.Controllers
{
    public class CartController : Controller
    {
        private readonly SalesManagementContext _dbContext;

        public CartController(SalesManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ProductId"] = productId;
                TempData["Quantity"] = quantity;
                return RedirectToAction("Login", "Members");
            }

            var email = User.Identity.Name;
            var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (member == null)
            {
                return Unauthorized();
            }

            var cartItem = await _dbContext.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.MemberId == member.Id);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    MemberId = member.Id
                };
                _dbContext.CartItems.Add(cartItem);
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added to cart successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the product to the cart.";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Index()
        {
            var email = User.Identity.Name;
            var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (member == null)
            {
                return RedirectToAction("Login", "Members");
            }

            var cartItems = await _dbContext.CartItems
                .Where(c => c.MemberId == member.Id)
                .Include(c => c.Product)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _dbContext.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _dbContext.CartItems.Remove(cartItem);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var email = User.Identity.Name;
            var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (member == null)
            {
                return Unauthorized();
            }

            var cartItems = await _dbContext.CartItems
                .Where(c => c.MemberId == member.Id)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Home");
            }

            var order = new Order
            {
                MemberId = member.Id,
                OrderDate = DateTime.UtcNow,
                RequiredDate = DateTime.UtcNow.AddDays(5),
                ShippedDate = DateTime.UtcNow.AddDays(1),
                Freight = 10,
                OrderDetails = cartItems.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice,
                    Discount = 0
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            _dbContext.CartItems.RemoveRange(cartItems);

            try
            {
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order placed successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while placing the order.";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}


