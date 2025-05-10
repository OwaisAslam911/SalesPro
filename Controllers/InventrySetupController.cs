using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SalesPro.Models;
using System.Data;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;



namespace SalesPro.Controllers
{
    public class InventrySetupController : Controller
    {
        private ILogger<InventrySetupController> _logger;
        private readonly IConfiguration configuration;

        public InventrySetupController(ILogger<InventrySetupController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }
        public IActionResult CategorySetup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveCategory([FromBody] Category category)
        {
            var con = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(con))
            {
                string sql = @"INSERT INTO Categories (CategoryCode, CategoryName, Abbreviation)
                       VALUES (@CategoryCode, @CategoryName, @Abbreviation)";
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql, category);

                if (result > 0)
                    return Json(new { success = true, message = "Category Saved Successfully!" });
                else
                    return Json(new { success = false, message = "Error Saving Category" });
            }
        }
        [HttpPost]
        public JsonResult UpdateCategory([FromBody] Category category)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string updateQuery = "Update  Categories set CategoryName = @categoryname,CategoryCode = @categoryCode,Abbreviation = @abbreviation Where CategoryId = 1001";
                connection.Execute(updateQuery, new { categoryId = category.CategoryId, categoryname = category.CategoryName, categoryCode = category.CategoryCode, abbreviation = category.Abbreviation });
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult DeleteCategory(string categoryId)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string deleteQuery = "Delete from Categories Where CategoryId = @categoryId";
                connection.Execute(deleteQuery, new { categoryId = categoryId });

                return Json(new { success = true });
            }
        }
            
        [HttpGet]
        public JsonResult GetCategories(Category category)
        {
            var con = configuration.GetConnectionString("dbcs");
            using(var connection = new SqlConnection(con))
            {
                var query = "Select * from Categories";
                var result = connection.Query(query).ToList();
            return Json(result);

            }
        }

        public IActionResult BrandSetup()
        {
            return View();
        }
        [HttpGet]
        public  JsonResult GetBrands(Brands brands)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                var query = "select * from Brands";
                var result = connection.Query(query).ToList();

            return Json(result);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveBrand([FromBody] Brands brand)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using(var connection = new SqlConnection(conn))
            {
                string query = "insert into Brands (BrandCode ,BrandName )values(@BrandCode ,@BrandName)";
                var result = await connection.ExecuteAsync(query, new { BrandName = brand.BrandName, BrandCode = brand.BrandCode });
                if (result > 0)
                    return Json(new { success = true, message = "Category Saved Successfully!" });
                else
                    return Json(new { success = false, message = "Error Saving Category" });
            }
        }
        [HttpPost]
        public JsonResult UpdateBrand([FromBody] Brands brand)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string query = "update Brands set BrandCode = @brandCode, BrandName = @brandName where BrandId = 1;";
                var result = connection.Execute(query, new { brandCode = brand.BrandCode, brandName = brand.BrandName});
                return Json(new { success = true });
            }
            
        }   
        public JsonResult DeleteBrand(string BrandId)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using(var connection = new SqlConnection(conn))
            {
                string query = "delete from Brands where BrandId = @BrandId";
                var result = connection.Execute(query, new { BrandId = BrandId });
                return Json(new { success = true });
            }
        }


        public IActionResult UnitSetup()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetUnits(Units unit)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string query = "Select * from Units";
                var result = connection.Query(query).ToList();
                return Json(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUnit([FromBody] Units unit)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using(var connection = new SqlConnection(conn))
            {
                var insertQuery = "Insert into Units ( UnitName, Abbreviation, UnitCode)Values(@unitName, @abbreviation, @unitCode)";
                var result = await connection.ExecuteAsync(insertQuery,new { unitCode = unit.UnitCode, unitName = unit.UnitName, abbreviation = unit.Abbreviation });
                if (result > 0)
                    return Json(new { success = true, message = "Unit of Measurment Saved Successfully!" });
                else
                    return Json(new { success = false, message = "Error Saving Unit of Measurment" });
            }
        }
        [HttpPost]
        public JsonResult UpdateUnit([FromBody]Units unit)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using( var connection = new SqlConnection(conn))
            {
                string query = "update Units set UnitName= @unitName, Abbreviation= @abbreviation, UnitCode = @unitCode where UnitId = @unitId;";
                var result = connection.Execute(query, new {unitName =  unit.UnitName, unitCode = unit.UnitCode, abbreviation = unit.Abbreviation });
                return Json(new { success = true });
            }
        }
        [HttpPost]
        public JsonResult DeleteUnit(string UnitId)
        {
            var conn = configuration.GetConnectionString("dbcs");
           using(var connection = new SqlConnection(conn))
            {
                string query = "Delete from Units where UnitId = @UnitId";
                var result = connection.Execute(query, new {UnitId = UnitId});
                return Json(new { success = true });
            }
        }

        public IActionResult ItemSetup()
        {
            var conn = configuration.GetConnectionString("dbcs");
            using( var connection = new SqlConnection(conn))
            {
                var categoryList = connection.Query<Category>("Select CategoryId, CategoryName from Categories").ToList();
                ViewBag.CategoryList = new SelectList(categoryList, "CategoryId", "CategoryName");

                var Unit = connection.Query<Units>("SELECT UnitId, UnitName from Units").ToList();
                ViewBag.UnitList = new SelectList(Unit, "UnitId", "UnitName");

                var BrandList = connection.Query<Brands>("Select BrandId, BrandName from Brands").ToList();
                ViewBag.BrandList = new SelectList(BrandList, "BrandId", "BrandName");
            }
            return View();
        }

        public  JsonResult GetItems(ItemTable item)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using(var connection = new SqlConnection(conn))
            {
                string query = "select * from Items i inner join  Categories c on i.CategoryId = c.CategoryId inner join Units u on i.UnitId = u.UnitId inner join Brands b on i.BrandId = b.BrandId";
                var result =  connection.Query(query).ToList();
                return Json(result);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveItem([FromBody] Items items)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string query = "Insert into Items(ItemCode, ItemName, StockType, CategoryId, Size, BrandId, Packing, SellingPrice, OpeningBalance, UnitId, CostPrice, Amount) values(@ItemCode, @ItemName, @StockType, @CategoryId, @Size, @BrandId, @Packing, @SellingPrice, @OpeningBalance, @UnitId, @CostPrice, @Amount)";
                var result = await connection.ExecuteAsync(query, new {ItemCode = items.ItemCode, ItemName = items.ItemName, StockType = items.StockType, CategoryId = items.CategoryId, Size = items.Size, BrandId = items.BrandId, Packing = items.Packing, SellingPrice = items.SellingPrice, OpeningBalance = items.OpeningBalance, UnitId = items.UnitId, CostPrice = items.CostPrice, Amount = items.Amount});
                if (result > 0)
                    return Json(new { success = true, message = "Unit of Measurment Saved Successfully!" });
                else
                    return Json(new { success = false, message = "Error Saving Unit of Measurment" });
            }
        }

        [HttpPost]
        public JsonResult UpdateItem([FromBody] Items items)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string query = "update Items set ItemName = @itemName, ItemCode = @itemCode, StockType = @stockType, CategoryId = @categoryId, Size = @size, BrandId = @brandId, Packing= @packing, SellingPrice= @salePrice, OpeningBalance = @openingBalance, UnitId = @unitId,CostPrice = @costPrice, Amount = @amount";
                var result = connection.Execute(query, new { itemName = items.ItemName, itemCode = items.ItemCode, stockType = items.StockType, categoryId = items.CategoryId, size = items.Size, brandId = items.BrandId, packing = items.Packing, salePrice = items.SellingPrice, openingBalance = items.OpeningBalance, unitId = items.UnitId, amount = items.Amount, costPrice = items.CostPrice });
            return Json(new {success = true});
            }

        }
        [HttpPost]
        public JsonResult DeleteItem(string ItemId)
        {
            var conn = configuration.GetConnectionString("dbcs");
            using(var connection = new SqlConnection(conn))
            {
                string query = "Delete from Items where ItemId = @ItemId";
                var result = connection.Execute(query, new { ItemId = ItemId });

                return Json(new { success = true });
            }
           
        }


    }
}
