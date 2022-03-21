using BarberStore.Infrastructure.Data.Enums;

namespace BarberStore.Core.Models.Appointments;

public class AppointmentModel : CalendarAppointmentViewModel
{
    public string? UserId { get; set; }
    public string[]? Services { get; set; }
}

public class CalendarAppointmentViewModel
{
    public DateTime Start { get; set; }
}

public class UserAppointmentViewModel : CalendarAppointmentViewModel
{
    public string? UserId { get; set; }
    public IList<ServiceModel> Services { get; set; } = new List<ServiceModel>();
    public Status Status { get; set; }
}