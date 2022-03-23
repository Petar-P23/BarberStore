namespace BarberStore.Core.Models.Appointments;

public class AppointmentsPageViewModel
{
    public IEnumerable<CalendarAppointmentViewModel> CalendarViewModels { get; set; }
    public IEnumerable<UserAppointmentViewModel> UserAppointmentViewModels { get; set; }
}