using BarberStore.Infrastructure.Data.Enums;

namespace BarberStore.Core.Models.Appointments;

public class UserAppointmentViewModel : CalendarAppointmentViewModel
{
    public string? UserId { get; set; }
    public IList<ServiceModel> Services { get; set; } = new List<ServiceModel>();
    public Status Status { get; set; }
}