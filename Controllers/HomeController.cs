using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SalesPro.Models;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace SalesPro.Controllers
{

    public class HomeController(ILogger<HomeController> logger, IConfiguration configuration,  IDbConnection db, SalesProContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IDbConnection db = db;
        private readonly IConfiguration configuration = configuration;
        private readonly SalesProContext context = context;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Customer addcustomer)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = "INSERT INTO Customers (CustomerName) VALUES (@CustomerName); SELECT CAST(SCOPE_IDENTITY() as int)";
                var id = connection.ExecuteScalar<int>(sql, addcustomer);
                addcustomer.CustomerId = id;
                return Json(addcustomer);
            }
        }
        public IActionResult GetAll()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(connectionString))
            {
                var customers = connection.Query<Customer>("SELECT * FROM customers").ToList();
                return Json(customers);
            }
        }
        public IActionResult AddProduct()
        {
            return View();
        }
        public IActionResult CreateProduct(Product product)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("dbcs")))
            {
                var sql = @"INSERT INTO Products (ProductName, Rate) 
                        VALUES (@ProductName, @Rate);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var id = connection.ExecuteScalar<int>(sql, product);
                product.ProductId = id;

                return Json(product); 
            }
        }

        public IActionResult GetAllProducts()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("dbcs")))
            {
                var products = connection.Query<Product>("SELECT * FROM Products").ToList();
                return Json(products);
            }
        }
        public IActionResult SalesForm()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("dbcs")))
            {
                var products = connection.Query<Product>("SELECT * FROM Products").ToList();
                ViewBag.Products = new SelectList(products, "ProductId", "ProductName");
                var customers = connection.Query<Customer
                    >("SELECT * FROM customers").ToList();
                ViewBag.Customers = new SelectList(customers, "CustomerId", "CustomerName");
            }

            return View();
        }
       
       

        [HttpPost]
        public IActionResult Save([FromBody] SaleDTO saleDto)
        {
            if (saleDto == null || saleDto.Products == null || !saleDto.Products.Any())
                return BadRequest("Invalid sale data.");

            var sale = new Sale
            {
                CustomerId = saleDto.CustomerId,
                SaleDate = saleDto.SaleDate,
                AddCharges = saleDto.AddCharges,
                Discount = saleDto.LessDiscount,
                GrossTotal = saleDto.Products.Sum(p => p.Qty * p.Rate),
                TotalQty = saleDto.Products.Sum(p => p.Qty)
            };

            var saleId = db.ExecuteScalar<int>(@"
        INSERT INTO Sales (CustomerId, SaleDate, TotalQty, GrossTotal, AddCharges, Discount)
        VALUES (@CustomerId, @SaleDate, @TotalQty, @GrossTotal, @AddCharges, @Discount);
        SELECT SCOPE_IDENTITY();", sale);

            foreach (var p in saleDto.Products)
            {
                db.Execute(@"INSERT INTO SaleDetails (SaleId, ProductId, Quantity, Rate, Amount)
                      VALUES (@SaleId, @ProductId, @Qty, @Rate, @Amount)",
                            new
                            {
                                SaleId = saleId,
                                ProductId = p.ProdId,
                                Qty = p.Qty,
                                Rate = p.Rate,
                                Amount = p.Qty * p.Rate
                            });
            }

            return Ok(saleId);
        }
        public IActionResult Bill(int id)
        {
            var sale = db.QueryFirstOrDefault(@"SELECT s.SaleId, s.SaleDate, c.CustomerName, 
                                         s.TotalQty, s.GrossTotal, s.AddCharges, s.Discount
                                         FROM Sales s
                                         JOIN Customers c ON c.CustomerId = s.CustomerId
                                         WHERE s.SaleId = @SaleId", new { SaleId = id });

            var items = db.Query<SaleItem>(@"SELECT p.ProductName, sd.Quantity, sd.Rate
                                      FROM SaleDetails sd
                                      JOIN Products p ON p.ProductId = sd.ProductId
                                      WHERE sd.SaleId = @SaleId", new { SaleId = id }).ToList();

            var viewModel = new SaleBillViewModel
            {
                SaleId = sale.SaleId,
                SaleDate = sale.SaleDate,
                CustomerName = sale.CustomerName,
                TotalQty = sale.TotalQty,
                GrossTotal = sale.GrossTotal,
                AddCharges = sale.AddCharges,
                Discount = sale.Discount,
                SaleItems = items
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
