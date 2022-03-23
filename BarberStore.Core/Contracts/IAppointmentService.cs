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
    public Task<(bool, string)> CreateAppointment(AppointmentModel model);

    /// <summary>
    /// Used to edit an appointment.
    /// </summary>
    /// <param name="userId">The user who owns the appointment.</param>
    /// <param name="appointmentId"></param>
    /// <param name="model">The model of the new appointment.</param>
    /// <returns>True if the edit is successful, false and an error message if it fails.</returns>
    public Task<(bool, string)> EditAppointment(string userId, string appointmentId, AppointmentModel model);
    /// <summary>
    /// Used to cancel an appointment.
    /// </summary>
    /// <param name="userId">The user who has the appointment.</param>
    /// <param name="appointmentId">The appointment to be cancelled.</param>
    /// <returns>True if cancellation is successful, false and an error message if it fails.</returns>
    public Task<(bool, string)> CancelAppointment(string userId, string appointmentId);
    /// <summary>
    /// Used to change an appointment's status.
    /// </summary>
    /// <param name="userId">The user who owns the appointment.</param>
    /// <param name="appointmentId">The appointment's id.</param>
    /// <param name="status">The new status of the appointment.</param>
    /// <returns>True if change is successful, false and an error message if it fails.</returns>
    public Task<(bool, string)> ChangeAppointmentStatus(string userId, string appointmentId, Status status);
    /// <summary>
    /// Used to get appointments for the calendar.
    /// </summary>
    /// <param name="month">Number of the month.</param>
    /// <returns>All appointments in a given month.</returns>
    public Task<IEnumerable<CalendarAppointmentViewModel>> GetAllAppointmentsByMonth(int month);
    /// <summary>
    /// Used to get all appointments that a given user has.
    /// </summary>
    /// <param name="userId">Id of the user.</param>
    /// <returns>The appointment with all services to do and their prices.</returns>
    public Task<IEnumerable<UserAppointmentViewModel>> GetUserAppointments(string userId);
    /// <summary>
    /// Used to get all appointments in the administration panel.
    /// </summary>
    /// <returns>All appointments with status pending and their services.</returns>
    public Task<IEnumerable<AdminPanelAppointmentViewModel>> GetAllPendingAppointments();
}