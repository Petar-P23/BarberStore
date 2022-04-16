using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private IStoreService storeService;
        private UserManager<ApplicationUser> userManager;
        private IAppointmentService appointmentService;

        public ManageController(IStoreService storeService,
            UserManager<ApplicationUser> userManager, 
            IAppointmentService appointmentService)
        {
            this.storeService = storeService;
            this.userManager = userManager;
            this.appointmentService = appointmentService;
        }

        public async Task<IActionResult> Orders()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var orders = await this.storeService.GetOrdersByUserAsync(user.Id);

            return View(orders);
        }

        public async Task<IActionResult> Appointments()
        {
            var userId = this.userManager.GetUserId(User);
            IEnumerable<UserAppointmentViewModel> appointments = 
                await this.appointmentService.GetUserAppointmentsAsync(userId);
            return View(appointments);
        }
    }
}
