using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRAssignment.Controllers
{
	public class StatisticOrdersController : Controller
	{
		private readonly SalesManagementContext _context;

		public StatisticOrdersController(SalesManagementContext context)
		{
			_context = context;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index()
		{

            var orders = await _context.Orders
				.Include(o => o.OrderDetails)
				.GroupBy(o => o.MemberId)
				.Select(g => new
				{
					MemberId = g.Key,
					MemberEmail = g.Select(o => o.Member.Email).FirstOrDefault(),
					TotalSpent = g.SelectMany(o => o.OrderDetails)
								  .Sum(od => od.UnitPrice * od.Quantity),
					Orders = g.SelectMany(o => o.OrderDetails)
				})
				.OrderByDescending(t => t.TotalSpent)
				.ToListAsync();


            var orderStatistics = orders.Select(o => new OrderStatistic
			{
				MemberId = o.MemberId,
				MemberEmail = o.MemberEmail,
				TotalSpent = o.TotalSpent,
				Orders = o.Orders
			}).ToList();


			double totalRevenue = orderStatistics.Sum(o => o.TotalSpent);
            int orderCount = orderStatistics.Count();
            var buyers = orderStatistics.ToDictionary(o => o.MemberId, o => (TotalSpent: o.TotalSpent, Email: o.MemberEmail));
            var topBuyer = buyers.OrderByDescending(b => b.Value).FirstOrDefault();

            var products = orders
				.SelectMany(o => o.Orders)
				.GroupBy(od => od.ProductId)
				.Select(g => new
				{
					ProductId = g.Key,
					TotalQuantity = g.Sum(od => od.Quantity)
				})
				.ToList();

			int topProduct = products.OrderByDescending(p => p.TotalQuantity).FirstOrDefault().ProductId;
            string topProductName = await _context.Products
                .Where(p => p.ProductId == topProduct)
                .Select(p => p.ProductName)
                .FirstOrDefaultAsync();

            var viewModel = new StatisticOrders
			{
				OrderCount = _context.Orders.Count(),
				TopBuyer = topBuyer.Key,
                TopBuyerEmail = buyers[topBuyer.Key].Email,
                TopBuyerAmount = buyers[topBuyer.Key].TotalSpent,
                TopProduct = topProductName,
				TopProductQuantity = products.FirstOrDefault(p => p.ProductId == topProduct)?.TotalQuantity ?? 0,
				TotalRevenue = totalRevenue.ToString("C"),
				OrderStatistics = orderStatistics
			};

			return View(viewModel);
		}
	}
}
