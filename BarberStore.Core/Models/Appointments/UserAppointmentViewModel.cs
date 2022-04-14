using BarberStore.Infrastructure.Data.Enums;

namespace BarberStore.Core.Models.Appointments;

public class UserAppointmentViewModel : CalendarAppointmentViewModel
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public Status Status { get; set; }
}