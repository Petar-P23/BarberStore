using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Announcements;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BarberStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServicesService _servicesService;
        private readonly IAnnouncementService _announcementService;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ILogger<HomeController> logger,
            IServicesService servicesService,
            IAnnouncementService announcementService,
            UserManager<ApplicationUser> userManager)
        {
            this._logger = logger;
            this._servicesService = servicesService;
            this._announcementService = announcementService;
            this.userManager = userManager;
        }

        public IActionResult Welcome()
        {
            return this.View();
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<AnnouncementViewModel> announcements = await this._announcementService.GetTopAnnouncementsAsync();
            return this.View(announcements);
        }
        public async Task<IActionResult> Services()
        {
            IEnumerable<ServiceModel> services = await this._servicesService.GetServicesAsync();
            return this.View(services);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}