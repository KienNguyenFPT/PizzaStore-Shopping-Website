using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;

namespace SignalRAssignment.Services
{
    public class SearchService
    {
        private readonly SalesManagementContext _context;
        public SearchService(SalesManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> SearchProFunc(string str)
        {
            IQueryable<Product> query = _context.Products.AsQueryable();

            List<Product> result;

			var (num1, num2) = ParseString(str);


			if (num2 == 0)
            {
                if (int.TryParse(str, out int productId))
                {
                    result = await query.Where(p => p.ProductId == productId).ToListAsync();
                }
                else
                {
                    var normalizedStr = str.ToUpper();
                    result = await query.Where(p => p.ProductName.ToUpper().Contains(normalizedStr)).ToListAsync();
                }
            }
            else
            {
                result = await query.Where(p => (p.UnitPrice >= num1 && p.UnitPrice <= num2) ||
                                            (p.UnitsInStock >= num1 && p.UnitsInStock <= num2)).ToListAsync();
            }
            return result;
        }

		public async Task<List<Product>> SearchProHomeFunc(string str)
		{
			IQueryable<Product> query = _context.Products.AsQueryable();

			List<Product> result;

			var (num1, num2) = ParseString(str);


			if (num2 == 0)
			{
				var normalizedStr = str.ToUpper();
				result = await query.Where(p => p.ProductName.ToUpper().Contains(normalizedStr)).ToListAsync();
			}
			else
			{
				result = await query.Where(p => (p.UnitPrice >= num1 && p.UnitPrice <= num2)).ToListAsync();
			}
			return result;
		}

		public (double num1, double num2) ParseString(string str)
		{
			double num1 = 0;
			double num2 = 0;

			try
			{
				if (str.Contains("-"))
				{
					string[] period = str.Split('-');
					if (period.Length == 2)
					{
						string[] part1 = System.Text.RegularExpressions.Regex.Split(period[0], @"\s*(?<=\d)\s*(?=\D)");
						string[] part2 = System.Text.RegularExpressions.Regex.Split(period[1], @"\s*(?<=\d)\s*(?=\D)");

						try
						{
							num1 = double.Parse(part1[0]);
							num2 = double.Parse(part2[0]);
						}
						catch (FormatException)
						{
							throw new Exception("Invalid format!");
						}

						if (part1.Length == 1 && part2.Length == 2)
						{
							switch (part2[1].ToLower())
							{
								case "k":
								case "thousand":
									if (num1 < num2) num1 *= 1000;
									num2 *= 1000;
									break;
								case "mil":
								case "milion":
									if (num1 < num2) num1 *= 1000000;
									num2 *= 1000000;
									break;
							}
						}
						else if (part1.Length == 2 && part2.Length == 2)
						{
							switch (part1[1].ToLower())
							{
								case "k":
								case "thousand":
									num1 *= 1000;
									break;
								case "mil":
								case "milion":
									num1 *= 1000000;
									break;
							}
							switch (part2[1].ToLower())
							{
								case "k":
								case "thousand":
									num2 *= 1000;
									break;
								case "mil":
								case "milion":
									num2 *= 1000000;
									break;
							}
						}

						if (num1 > num2 || num1 < 0 || num2 <= 0)
						{
							throw new Exception("Invalid range!");
						}
					}
				}
			}
			catch (Exception ex)
			{
				num1 = 0;
				num2 = 0;
			}

			return (num1, num2);
		}

	}
}
