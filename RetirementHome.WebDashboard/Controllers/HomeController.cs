using Microsoft.AspNetCore.Mvc;
using RetirementHome.WebDashboard.Abstractions;
using RetirementHome.WebDashboard.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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

        [Authorize]
        public IActionResult Restricted()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Action to issue a challange to google login
        public IActionResult Google(string provider)
        {
            provider = "Google";
            //Issue a challenge to external login middleware to trigger sign in process
            //return new ChallengeResult(provider);

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("googlesignin")
            };

            return Challenge(authenticationProperties, "Google");
        }

        //Callback action to retrive signin user details  https://localhost:7190/home/googlesignin
        public Task<IActionResult> googlesignin(string returnUrl = null, string remoteError = null)
        {
            //Here we can retrieve the claims
            var result = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return null;
        }

    }
}