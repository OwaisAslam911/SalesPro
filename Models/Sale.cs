using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace SalesPro.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? SaleDate { get; set; }

    public string? Description { get; set; }

    public int? TotalQty { get; set; }

    public decimal? GrossTotal { get; set; }

    public decimal? AddCharges { get; set; }

    public decimal? Discount { get; set; }

    public decimal? NetTotal { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}

public class SaleViewModel
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal GrossTotal { get; set; }
    public decimal AddCharges { get; set; }
    public decimal Discount { get; set; }
    public List<SelectListItem> Customers { get; set; }
    public List<SelectListItem> Products { get; set; }
    public List<SaleDetail> SaleDetails { get; set; }
}


public class SaleBillViewModel
{
    public int SaleId { get; set; }
    public string CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
    public int TotalQty { get; set; }
    public decimal GrossTotal { get; set; }
    public decimal AddCharges { get; set; }
    public decimal Discount { get; set; }
    public decimal NetTotal => GrossTotal + AddCharges - Discount;

    public List<SaleItem> SaleItems { get; set; }
}

public class SaleItem
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount => Quantity * Rate;
}
public class SaleInfoData
{
    public int SaleId { get; set; }
    public string CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
    public int TotalQty { get; set; }
    public decimal GrossTotal { get; set; }
    public decimal AddCharges { get; set; }
    public decimal Discount { get; set; }
    public decimal NetTotal => GrossTotal + AddCharges - Discount;
}

public class SaleItemData
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount => Quantity * Rate;
}
public class ReportDataModel
{
    public string CustomerName { get; set; }
    public string SaleDate { get; set; }
    public List<ProductItem> Products { get; set; }
    public int TotalQty { get; set; }
    public decimal GrossTotal { get; set; }
    public decimal AddCharges { get; set; }
    public decimal LessDiscount { get; set; }
    public decimal NetTotal { get; set; }
}

public class ProductItem
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}