using BarberStore.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class AppointmentsController : BaseController
    {
        private readonly IAppointmentService appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            this.appointmentService = appointmentService;
        }

        public IActionResult Manage()
        {
            return this.View();
        }
        [HttpPost]
        public async Task<IActionResult> Manage(DateTime date)
        {
            var appointments = await this.appointmentService.GetAllAppointmentsByDateAsync(date);
            return this.View("Manage",appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(string id, string userId)
        {
            await this.appointmentService.CancelAppointmentAsync(userId, id);

            return RedirectToAction("Manage");
        }
    }
}
