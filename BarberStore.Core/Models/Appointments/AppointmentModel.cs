namespace BarberStore.Core.Models.Appointments;

public class AppointmentModel : CalendarAppointmentViewModel
{
    public string? UserId { get; set; }
    public string[]? Services { get; set; }
}