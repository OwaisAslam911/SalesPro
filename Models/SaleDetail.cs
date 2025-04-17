using System;
using System.Collections.Generic;

namespace SalesPro.Models;

public partial class SaleDetail
{
    public int SaleDetailId { get; set; }

    public int? SaleId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Rate { get; set; }

    public decimal? Amount { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Sale? Sale { get; set; }
}
public class SaleDTO
{
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
    public decimal AddCharges { get; set; }
    public decimal LessDiscount { get; set; }
    public List<SaleProductDTO> Products { get; set; }
}

public class SaleProductDTO
{
    public int ProdId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}
