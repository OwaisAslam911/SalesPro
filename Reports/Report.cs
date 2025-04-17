using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using SalesPro.Models;
using System.Reflection;

namespace SalesPro.Reports
{
    public class Report
    {
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

    }
}
