using BarberStore.Infrastructure.Data.Enums;

namespace BarberStore.Core.Models.Appointments;

public class AdminPanelAppointmentViewModel : CalendarAppointmentViewModel
{
    public string? UserId { get; set; }
    public Status Status { get; set; }
}