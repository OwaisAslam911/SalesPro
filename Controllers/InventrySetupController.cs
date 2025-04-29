using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SalesPro.Models;
using System.Data;
using System.Runtime.CompilerServices;



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
            return View();
        }
    }
}
