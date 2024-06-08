using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using ScadaPlus.Data.Repositories;

namespace ScadaPlus.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportController(IServiceProvider serviceProvider, IWebHostEnvironment webHostEnvironment)
        {
            _serviceProvider = serviceProvider;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Machines()
        {
            string reportPath = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "report-template.frx");

            var report = FastReport.Report.FromFile(reportPath);

            using (var scope = _serviceProvider.CreateScope())
            {
                var machineRepository = scope.ServiceProvider.GetRequiredService<MachineRepository>();
                report.RegisterData(await machineRepository.ReadMachinesAsync(), "Machines");
            }

            report.SetParameterValue("CustomParameter", "K. Tee");

            var webReport = new WebReport();

            webReport.Report = report;

            return View(webReport);
        }
    }
}
