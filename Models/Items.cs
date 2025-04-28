namespace SalesPro.Models
{
    public class Items
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode{ get; set; }
        public string StockType { get; set; }
        public int CategoryId { get; set; }
        public string Size { get; set; }
        public int BrandId { get; set; }
        public string Packing { get; set; }
        public decimal SellingPrice { get; set; }
        public int OpeningBalance { get; set; }
        public int UnitId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Amount { get; set; }

    }
}
