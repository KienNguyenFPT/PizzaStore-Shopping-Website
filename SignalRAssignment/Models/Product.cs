using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRAssignment.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Weight { get; set; } = null!;

    public int UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public string? Image { get; set; }
    public int SupplierId { get; set; }

    [NotMapped]
    public virtual Supplier Supplier { get; set; }

    [NotMapped]
    public virtual Category Category { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
