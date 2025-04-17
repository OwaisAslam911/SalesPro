using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using SalesPro.Models;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Reflection;
using System.Text;
namespace SalesPro.Controllers
{

    public class HomeController(ILogger<HomeController> logger, IConfiguration configuration,  IDbConnection db, SalesProContext context, IWebHostEnvironment _env) : Controller
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
                GrossTotal = saleDto.Products.Sum(p => p.Quantity * p.Rate),
                TotalQty = saleDto.Products.Sum(p => p.Quantity)
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
                                Qty = p.Quantity,
                                Rate = p.Rate,
                                Amount = p.Quantity * p.Rate
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
        public static void Load(LocalReport report, ReportDataModel data)
        {
            // Set up the data source from ProductItem list
            var items = data.Products;

            // Report parameters
            var parameters = new[]
            {
        new ReportParameter("CustomerName", data.CustomerName),
        new ReportParameter("SaleDate", data.SaleDate),
        new ReportParameter("TotalQty", data.TotalQty.ToString()),
        new ReportParameter("GrossTotal", data.GrossTotal.ToString("F2")),
        new ReportParameter("AddCharges", data.AddCharges.ToString("F2")),
        new ReportParameter("LessDiscount", data.LessDiscount.ToString("F2")),
        new ReportParameter("NetTotal", data.NetTotal.ToString("F2")),
    };

            // Load the RDLC file from embedded resource
            using var rs = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SalesPro.Reports.SaleInoice.rdl"); // make sure the path is correct
            report.LoadReportDefinition(rs);

            // Bind data source
            report.DataSources.Add(new ReportDataSource("ProductDataSet", items)); // Name must match RDLC dataset

            // Set parameters
            report.SetParameters(parameters);
        }

        [HttpPost]
        public IActionResult ViewReport([FromBody] ReportDataModel reportData)
        {
            // Ensure CodePages provider is registered (if not already registered at application startup)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            LocalReport report = new LocalReport();
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "SaleInvoice.rdl");
            report.ReportPath = reportPath;

            var flatData = reportData.Products.Select(product => new {
                CustomerName = reportData.CustomerName,
                SaleDate = reportData.SaleDate,
                TotalQty = reportData.TotalQty,
                GrossTotal = reportData.GrossTotal,
                AddCharges = reportData.AddCharges,
                LessDiscount = reportData.LessDiscount,
                NetTotal = reportData.NetTotal,
                ProductName = product.ProductName,
                Quantity = product.Quantity,
                Rate = product.Rate,
                Amount = product.Amount
            }).ToList();

            report.DataSources.Add(new ReportDataSource("SaleInvoice", flatData));
            report.Refresh();

            string deviceInfo = @"<DeviceInfo>
                          <HTMLFragment>True</HTMLFragment>
                      </DeviceInfo>";
            string mimeType, encoding, extension;
            Warning[] warnings;
            string[] streamIds;

            byte[] reportBytes = report.Render(
                "HTML5",
                deviceInfo,
                out mimeType,
                out encoding,
                out extension,
                out streamIds,
                out warnings
            );

            // Fix encoding issue
            if (encoding.Equals("Unicode (UTF-8)", StringComparison.OrdinalIgnoreCase))
            {
                encoding = "UTF-8";
            }

            string htmlContent = Encoding.GetEncoding(encoding).GetString(reportBytes);
            return Json(new { reportHtml = htmlContent });
        }

        //public IActionResult ViewReport()
        //{
        //    // Create a new instance of LocalReport
        //    LocalReport report = new LocalReport();

        //    // Set the path to the main RDLC report file
        //    string mainReportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "Invoice.rdl");
        //    report.ReportPath = mainReportPath;

        //    // Prepare your report data
        //    var reportData = GetReportData(); // Assume this returns a ReportDataModel instance

        //    // Flatten into a list where each product record also includes header info.
        //    var flatData = reportData.Products.Select(product => new
        //    {
        //        // Header fields
        //        CustomerName = reportData.CustomerName,
        //        SaleDate = reportData.SaleDate,
        //        TotalQty = reportData.TotalQty,
        //        GrossTotal = reportData.GrossTotal,
        //        AddCharges = reportData.AddCharges,
        //        LessDiscount = reportData.LessDiscount,
        //        NetTotal = reportData.NetTotal,

        //        // Detail fields
        //        ProductName = product.ProductName,
        //        Quantity = product.Quantity,
        //        Rate = product.Rate,
        //        // You can let the report compute the Amount by using its expression or pass it directly:
        //        Amount = product.Amount // or product.Quantity * product.Rate
        //    }).ToList();


        //    // Add the main report data source.
        //    // The name "ReportDataModel" should match the dataset name in your RDLC file.
        //    report.DataSources.Add(new ReportDataSource("SaleInvoice", flatData));


        //    // If you are using a subreport for products, hook into the SubreportProcessing event.
        //    report.SubreportProcessing += (sender, e) =>
        //    {
        //        // The subreport name should match the one defined in your main report
        //        // For instance, if your subreport is for product items, you might check e.ReportPath or a parameter.
        //        if (e.ReportPath.Equals("ProductsSubreport", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Supply the products list as the data source for the subreport.
        //            report.DataSources.Add(new ReportDataSource("SaleInvoice", flatData));

        //        }
        //    };

        //    // Render the report to PDF (or another format like HTML5)
        //    byte[] reportBytes = report.Render("PDF");

        //    // Return the PDF as a FileResult so it can be viewed or downloaded in the browser
        //    return File(reportBytes, "application/pdf", "SalesReport.pdf");
        //}
        private ReportDataModel GetReportData()
        {
            return new ReportDataModel
            {
                CustomerName = "John Doe",
                SaleDate = DateTime.Now.ToShortDateString(),
                TotalQty = 10,
                GrossTotal = 1000m,
                AddCharges = 50m,
                LessDiscount = 25m,
                NetTotal = 1025m,
                Products = new List<ProductItem>
            {
                new ProductItem { ProductName = "Product A", Quantity = 2, Rate = 100m, Amount = 200m },
                new ProductItem { ProductName = "Product B", Quantity = 3, Rate = 150m, Amount = 450m },
                new ProductItem { ProductName = "Product C", Quantity = 5, Rate = 70m, Amount = 350m }
            }
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
