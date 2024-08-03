using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRAssignment.Models;

public partial class Member
{
    public int Id { get; set; }

    public string Password { get; set; } = null!;

    public string Avt { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Hobby { get; set; } = null!;

    public Boolean Type { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
