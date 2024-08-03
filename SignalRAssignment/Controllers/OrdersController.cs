using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;
using SignalRAssignment.Services;
using static NuGet.Packaging.PackagingConstants;

namespace SignalRAssignment.Controllers
{
    public class OrdersController : Controller
    {
        private readonly SalesManagementContext _context;
        public List<OrderDetail> ListOrderDetails { get; set; }
        public OrdersController(SalesManagementContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            List<Order> orders = [];
            int totalOrderss = 0;
            if (User.IsInRole("Staff"))
            {
                orders = await _context.Orders.Include(o => o.Member).OrderByDescending(o => o.OrderDate).Skip((pageNumber - 1) * 10).Take(10).ToListAsync();

                totalOrderss = await _context.Orders.CountAsync();
            }
            else if (User.IsInRole("User"))
            {
                var memberIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                orders = await _context.Orders.Where(m => m.MemberId == memberIdClaim).Include(o => o.Member).OrderByDescending(o => o.OrderDate).Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
                totalOrderss = await _context.Orders.Where(m => m.MemberId == memberIdClaim).CountAsync();
            }


            var viewModel = new PaginatedList<Order>(orders, totalOrderss, pageNumber, 10);

            return View(viewModel);
        }
        
        public void UpdateOrder(OrderDetail OrderDetail)
        {
            foreach(OrderDetail o in ListOrderDetails)
            {
                if (o.ProductId == OrderDetail.ProductId)
                {
                    o.Quantity += OrderDetail.Quantity;
                    return;
                }
            }
            ListOrderDetails.Add(OrderDetail);
        }

        internal void DeleteItem(int id)
        {
            ListOrderDetails.Remove(ListOrderDetails.Where(x => x.ProductId == id).FirstOrDefault());
        }

        internal List<OrderDetail> ViewOrderDetail(int id)
        {
            return _context.Orders.Where(x => x.OrderId == id).Include(r => r.OrderDetails).FirstOrDefault().OrderDetails.ToList();
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Member)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            order.OrderDetails = await _context.OrderDetails.Where(o => o.OrderId == order.OrderId).Include(p => p.Product).ToListAsync();
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", order.MemberId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", order.MemberId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }
            ModelState.Remove("Member");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", order.MemberId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Member)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'SalesManagementContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }

	}
}
