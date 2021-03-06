using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Enums;

namespace BarberStore.Core.Contracts;

public interface IAppointmentService
{

    /// <summary>
    /// Used to create an appointment.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>True if model creation is successful, false and an error message if it fails.</returns>
    public Task<(bool, string)> CreateAppointmentAsync(AppointmentModel model);

    /// <summary>
    /// Used to cancel an appointment.
    /// </summary>
    /// <param name="userId">The user who has the appointment.</param>
    /// <param name="appointmentId">The appointment to be cancelled.</param>
    /// <returns>True if cancellation is successful, false and an error message if it fails.</returns>
    public Task<(bool, string)> CancelAppointmentAsync(string userId, string appointmentId);
    /// <summary>
    /// Used to change an appointment's status.
    /// </summary>
    /// <param name="userId">The user who owns the appointment.</param>
    /// <param name="appointmentId">The appointment's id.</param>
    /// <param name="status">The new status of the appointment.</param>
    /// <returns>True if change is successful, false and an error message if it fails.</returns>
    public Task<(bool, string)> ChangeAppointmentStatusAsync(string userId, string appointmentId, Status status);
    /// <summary>
    /// Used to get appointments for the calendar.
    /// </summary>
    /// <param name="month">Number of the month.</param>
    /// <returns>All appointments in a given month.</returns>
    public Task<IEnumerable<CalendarAppointmentViewModel>> GetAllAppointmentsByMonthAsync(int month);
    /// <summary>
    /// Used to get all appointments that a given user has.
    /// </summary>
    /// <param name="userId">Id of the user.</param>
    /// <returns>The appointment with all services to do and their prices.</returns>
    public Task<IEnumerable<UserAppointmentViewModel>> GetUserAppointmentsAsync(string userId);
    /// <summary>
    /// Used to get all appointments in the administration panel.
    /// </summary>
    /// <returns>All appointments with status pending and their services.</returns>
    public Task<IEnumerable<AdminPanelAppointmentViewModel>> GetAllAppointmentsByDateAsync(DateTime date);

    public Task<CalendarAppointmentViewModel> GetPreviousFreeAppointmentAsync(DateTime date, int workStart, int workEnd);
    public Task<CalendarAppointmentViewModel> GetNextFreeAppointmentAsync(DateTime date, int workStart, int workEnd);
    public Task<bool> CheckIfAppointmentExistsAsync(DateTime date);
}