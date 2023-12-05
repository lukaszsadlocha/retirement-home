using Microsoft.AspNetCore.Mvc;
using RetirementHome.WebDashboard.Abstractions;
using RetirementHome.WebDashboard.Models;
using System.Diagnostics;

namespace RetirementHome.WebDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IResidentsRepository _residentsRepository;

        public HomeController(ILogger<HomeController> logger, IResidentsRepository residentsRepository)
        {
            _logger = logger;
            this._residentsRepository = residentsRepository;
        }

        public IActionResult Index()
        {
            var residents = _residentsRepository.GetResidents();
            return View(residents);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}