using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService appointmentService;
        private readonly UserManager<ApplicationUser> userManager;

        public AppointmentsController(IAppointmentService appointmentService,
            UserManager<ApplicationUser> userManager)
        {
            this.appointmentService = appointmentService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Calendar(int month)
        {
            IEnumerable<CalendarAppointmentViewModel> appointments;
            IEnumerable<UserAppointmentViewModel> userAppointments;
            try
            {
                appointments = await this.appointmentService.GetAllAppointmentsByMonth(month);
                var user = this.userManager.GetUserId(this.User);

                userAppointments = await this.appointmentService.GetUserAppointments(user);
            }
            catch (Exception)
            {
                return this.BadRequest();
            }

            var pageModel = new AppointmentsPageViewModel
            {
                CalendarViewModels = appointments,
                UserAppointmentViewModels = userAppointments
            };

            return this.View(pageModel);
        }

        public async Task<IActionResult> Cancel(string id)
        {
            var user = this.userManager.GetUserId(this.User);

            var (success, error) = await this.appointmentService.CancelAppointment(user, id);
            if (!success)
            {
                return this.BadRequest(error);
            }

            var month = DateTime.Now.Month;
            return this.RedirectToAction("Calendar", month);
        }

        public async Task<IActionResult> Create(string[] serviceIds, DateTime appointmentTime)
        {
            var user = this.userManager.GetUserId(this.User);

            var appointment = new AppointmentModel
            {
                Start = appointmentTime,
                UserId = user,
                Services = serviceIds
            };

            var (success, error) = await this.appointmentService.CreateAppointment(appointment);
            if (!success)
            {
                return this.BadRequest(error);
            }

            var month = DateTime.Now.Month;
            return this.RedirectToAction("Calendar", month);
        }

        public async Task<IActionResult> Edit(string id, string[] serviceIds, DateTime appointmentTime)
        {
            var user = this.userManager.GetUserId(this.User);

            var appointment = new AppointmentModel
            {
                Start = appointmentTime,
                UserId = user,
                Services = serviceIds
            };

            var (success, error) = await this.appointmentService.EditAppointment(user, id, appointment);
            if (!success)
            {
                return this.BadRequest(error);
            }

            var month = DateTime.Now.Month;
            return this.RedirectToAction("Calendar", month);
        }
    }
}
