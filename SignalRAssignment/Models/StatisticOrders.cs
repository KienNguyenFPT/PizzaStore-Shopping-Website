namespace SignalRAssignment.Models
{
	public class StatisticOrders
	{
		public int OrderCount { get; set; }
		public int TopBuyer { get; set; }
		public string TopBuyerEmail { get; set; }
		public double TopBuyerAmount { get; set; }
		public string TopProduct { get; set; }
		public double TopProductQuantity { get; set; }
		public string TotalRevenue { get; set; }
		public IEnumerable<OrderStatistic> OrderStatistics { get; set; }
	}
	public class OrderStatistic
	{
		public int MemberId { get; set; }
		public string MemberEmail { get; set; }
		public double TotalSpent { get; set; }
		public IEnumerable<OrderDetail> Orders { get; set; }
	}
}
	