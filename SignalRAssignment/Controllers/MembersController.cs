using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SignalRAssignment.Models;
using SignalRAssignment.Services;

namespace SignalRAssignment.Controllers
{
    public class MembersController : Controller
    {
        private readonly SalesManagementContext _context;
        private readonly TokenService _tokenService;
        private readonly UploadImagesService _uploadImagesService;

        public Member member { get; set; }

        public MembersController(TokenService tokenService, UploadImagesService uploadImagesService, SalesManagementContext context)
        {
            _tokenService = tokenService;
            _context = context;
            _uploadImagesService = uploadImagesService;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
              return _context.Members != null ? 
                          View(await _context.Members.ToListAsync()) :
                          Problem("Entity set 'SalesManagementContext.Members'  is null.");
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Password,Avt,Name,Birthday,Email,Phone,City,Country,Hobby")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] Member member)
        {
            if (member.Email == null || _context.Members == null)
            {
                return NotFound();
            }
            Member check = _context.Members.Where(m => m.Email.Equals(member.Email)).FirstOrDefault();
            if (check != null && check.Password.Equals(member.Password))
            {
                return RedirectToAction("Index");
            }
            return View(member);
        } */

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Password,Avt,Name,Birthday,Email,Phone,City,Country,Hobby,ImageFile,Type")] Member member)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Avt");
            if (ModelState.IsValid)
            {
                try
                {
                    if (member.ImageFile != null)
                    {
                        member.Avt = "images/" + _uploadImagesService.uploadImage(member.ImageFile, "images/");
                    }
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
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
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Members == null)
            {
                return Problem("Entity set 'SalesManagementContext.Members'  is null.");
            }
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
          return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] Member m)
        {
            var member = await ValidateMemberAsync(m.Email, m.Password);
            if (member == null)
            {
                ViewData["ErrorMessage"] = "ID/Password not correct!";
                return View();
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Email),
                new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                new Claim(ClaimTypes.Role, member.Type ? "Staff": "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "SaleAppSaintRai");

            await HttpContext.SignInAsync("SaleAppSaintRai", new ClaimsPrincipal(claimsIdentity));

            int? productId = TempData["ProductId"] as int?;
            int? quantity = TempData["Quantity"] as int?;

            if (productId.HasValue && quantity.HasValue)
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.ProductId == productId.Value && c.MemberId == member.Id);

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity.Value;
                }
                else
                {
                    cartItem = new CartItem
                    {
                        ProductId = productId.Value,
                        Quantity = quantity.Value,
                        MemberId = member.Id
                    };
                _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added to cart successfully!";

                HttpContext.Session.Remove("ProductId");
                HttpContext.Session.Remove("Quantity");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("SaleAppSaintRai");
            return RedirectToAction("Index", "Home");
        }

        private async Task<Member> ValidateMemberAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var member = await _context.Members.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (member == null)
            {
                return null;
            }

            return member;
        }

    }
}
