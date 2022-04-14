namespace BarberStore.Core.Models.Appointments;

public class AppointmentsPageViewModel
{
    public CalendarAppointmentViewModel Previous { get; set; }
    public CalendarAppointmentViewModel Next { get; set; }
    public CalendarAppointmentViewModel Current { get; set; }
    public IEnumerable<UserAppointmentViewModel> UserAppointmentViewModels { get; set; }
}