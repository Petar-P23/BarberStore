using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Controllers
{
    [Authorize]
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
        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            var userId = this.userManager.GetUserId(User);
            var userAppointments =
                await this.appointmentService.GetUserAppointments(userId);

            return View(new AppointmentsPageViewModel
            {
                Previous = null,
                Next = null,
                Current = null,
                UserAppointmentViewModels = userAppointments
            });
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(DateTime date, string time)
        {
            var timeParts = time.Split(":").Select(int.Parse).ToArray();
            date = new DateTime(date.Year, date.Month, date.Day, timeParts[0], timeParts[1], 0);

            var userId = this.userManager.GetUserId(User);
            var userAppointments =
                await this.appointmentService.GetUserAppointments(userId);

            if (await this.appointmentService.CheckIfAppointmentExists(date))
            {
                var previous =
                    await this.appointmentService.GetPreviousFreeAppointment(date, 9, 18);
                var next =
                    await this.appointmentService.GetNextFreeAppointment(date, 9, 18);

                return View(new AppointmentsPageViewModel
                {
                    Previous = previous,
                    Next = next,
                    Current = null,
                    UserAppointmentViewModels = userAppointments
                });
            }

            var pageModel = new AppointmentsPageViewModel
            {
                Previous = null,
                Next = null,
                Current = new CalendarAppointmentViewModel() { Start = date },
                UserAppointmentViewModels = userAppointments
            };

            if (date < DateTime.Now)
            {
                pageModel.Next = await this.appointmentService.GetNextFreeAppointment(DateTime.Now, 9, 18);
                pageModel.Current = null;
            }
            return View(pageModel);
        }
        [HttpGet]
        public async Task<IActionResult> Cancel(string id, string returnUrl)
        {
            var user = this.userManager.GetUserId(this.User);

            var (success, error) = await this.appointmentService.CancelAppointment(user, id);
            if (!success)
            {
                return this.BadRequest(error);
            }

            if (returnUrl != null)
                return Redirect(returnUrl);

            return this.RedirectToAction("BookAppointment");
        }
        [HttpPost]
        public async Task<IActionResult> Create(DateTime date, string time)
        {
            var user = this.userManager.GetUserId(this.User);
            var timeParts = time.Split(":").Select(int.Parse).ToArray();
            var appointmentTime = new DateTime(date.Year, date.Month, date.Day, timeParts[0], timeParts[1], 0);

            var appointment = new AppointmentModel
            {
                Start = appointmentTime,
                UserId = user,
            };

            var (success, error) = await this.appointmentService.CreateAppointment(appointment);
            if (!success)
            {
                return this.BadRequest(error);
            }

            return this.RedirectToAction("BookAppointment");
        }

        //public async Task<IActionResult> Edit(string id, string[] serviceIds, DateTime appointmentTime)
        //{
        //    var user = this.userManager.GetUserId(this.User);

        //    var appointment = new AppointmentModel
        //    {
        //        Start = appointmentTime,
        //        UserId = user,
        //        Services = serviceIds
        //    };

        //    var (success, error) = await this.appointmentService.EditAppointment(user, id, appointment);
        //    if (!success)
        //    {
        //        return this.BadRequest(error);
        //    }

        //    var month = DateTime.Now.Month;
        //    return this.RedirectToAction("BookAppointment");
        //}
    }
}
