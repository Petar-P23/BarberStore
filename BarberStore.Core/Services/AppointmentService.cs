using BarberStore.Core.Common;
using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using static BarberStore.Core.Constants.ExceptionMessageConstants;

namespace BarberStore.Core.Services;

public class AppointmentService : DataService, IAppointmentService
{
    public AppointmentService(IApplicationDbRepository repo)
        : base(repo)
    {

    }
    public async Task<(bool, string)> CreateAppointmentAsync(AppointmentModel model)
    {
        try
        {
            Guard.AgainstNull(model);
            if (model.Start.Day < DateTime.Today.Day)
            {
                throw new ArgumentException(AppointmentInvalidDateException);
            }

            var appointment = new Appointment
            {
                Start = model.Start,
                UserId = model.UserId,
                Status = Status.Pending
            };

            await this.repo.AddAsync(appointment);
            await this.repo.SaveChangesAsync();
        }
        catch (ArgumentException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
    }

    public async Task<(bool, string)> ChangeAppointmentStatusAsync(string userId, string appointmentId, Status status)
    {
        try
        {
            var appointment = await this.repo
                .All<Appointment>()
                .Where(a => a.UserId == userId && a.Id == Guid.Parse(appointmentId))
                .FirstOrDefaultAsync();
            Guard.AgainstNull(appointment);

            appointment.Status = status;

            await this.repo.SaveChangesAsync();
        }
        catch (ArgumentException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
    }
    public async Task<(bool, string)> CancelAppointmentAsync(string userId, string appointmentId)
    {
        return await ChangeAppointmentStatusAsync(userId, appointmentId, Status.Cancelled);
    }
    public async Task<IEnumerable<CalendarAppointmentViewModel>> GetAllAppointmentsByMonthAsync(int month)
    {
        Guard.AgainstOutOfRange(month, 1, 12, nameof(month));
        var appointments = await this.repo.All<Appointment>()
            .Where(a => a.Status != Status.Cancelled && a.Start.Month == month)
            .Select(a => new CalendarAppointmentViewModel { Start = a.Start })
            .OrderBy(a => a.Start.Day)
            .ToListAsync();

        return appointments;
    }
    public async Task<IEnumerable<UserAppointmentViewModel>> GetUserAppointmentsAsync(string userId)
    {
        Guard.AgainstNullOrWhiteSpaceString(userId, nameof(userId));
        var appointments = await this.repo.All<Appointment>()
            .Where(a => a.UserId == userId)
            .Select(a => new UserAppointmentViewModel
            {
                Id = a.Id.ToString(),
                Start = a.Start,
                UserId = a.UserId,
                Status = a.Status
            })
            .OrderByDescending(a => a.Status)
            .ThenByDescending(a => a.Start)
            .ToListAsync();

        return appointments;
    }
    public async Task<IEnumerable<AdminPanelAppointmentViewModel>> GetAllAppointmentsByDateAsync(DateTime date)
    {
        var appointments = await this.repo.All<Appointment>()
            .Where(a => a.Start.Date == date.Date)
            .Select(a => new AdminPanelAppointmentViewModel
            {
                Id = a.Id.ToString(),
                UserId = a.UserId,
                Start = a.Start,
                UserFirstName = a.User.FirstName,
                UserLastName = a.User.LastName,
                Status = a.Status
            })
            .OrderByDescending(a => a.Status)
            .ThenByDescending(a => a.Start)
            .ToListAsync();

        return appointments;
    }
    public async Task<CalendarAppointmentViewModel> GetPreviousFreeAppointmentAsync(DateTime date, int workStart, int workEnd)
    {
        if (workStart == workEnd || workStart == 0 || workEnd == 0)
        {
            return null;
        }

        while (await CheckIfAppointmentExistsAsync(date))
        {
            date = date.AddMinutes(-30);
            if (date.Hour >= workStart) continue;
            date = date.AddDays(-1);
            date = date.AddHours(9);
        }

        if (date < DateTime.Now) return null;
        return new CalendarAppointmentViewModel { Start = date };
    }
    public async Task<CalendarAppointmentViewModel> GetNextFreeAppointmentAsync(DateTime date, int workStart, int workEnd)
    {
        if (workStart == workEnd || workStart == 0 || workEnd == 0)
        {
            return null;
        }

        if (date.Minute is not (30 or 0) || date < DateTime.Now)
        {
            var now = DateTime.Now;
            date = new DateTime(now.Year, now.Month, now.Day, now.Hour, 30, 0);
            if (date < now)
            {
                date = date.AddMinutes(30);
            }

            if (date.Hour >= workEnd)
            {
                date = date.AddDays(1);
                date = date.AddHours((date.Hour - workStart) * -1);
            }

            if (date.Hour <= workStart)
            {
                date = date.AddHours(workStart - date.Hour);
            }
        }

        while (await CheckIfAppointmentExistsAsync(date))
        {
            date = date.AddMinutes(30);
            if (date.Hour < workEnd) continue;
            date = date.AddDays(1);
            date = date.AddHours((date.Hour - workStart) * -1);
        }

        return new CalendarAppointmentViewModel { Start = date };
    }

    public async Task<bool> CheckIfAppointmentExistsAsync(DateTime date)
    {
        var exists = await this.repo.All<Appointment>()
            .Where(a => a.Start == date && a.Status != Status.Cancelled)
            .FirstOrDefaultAsync() != null;
        return exists;
    }
}
