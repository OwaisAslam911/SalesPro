using System;
using System.Collections.Generic;

namespace SalesPro.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    //public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
