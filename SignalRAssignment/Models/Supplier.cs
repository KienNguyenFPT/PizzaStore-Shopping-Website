namespace SignalRAssignment.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        public string CompanyName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
