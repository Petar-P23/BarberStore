using BarberStore.Infrastructure.Data.Enums;

namespace BarberStore.Core.Models.Appointments;

public class AdminPanelAppointmentViewModel : CalendarAppointmentViewModel
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public Status Status { get; set; }
}