using BarberStore.Core.Common;
using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static BarberStore.Core.Constants.ExceptionMessageConstants;

namespace BarberStore.Core.Services;

public class AppointmentService : DataService, IAppointmentService
{
    public AppointmentService(IApplicationDbRepository repo)
        : base(repo)
    {

    }
    public async Task<(bool, string)> CreateAppointment(AppointmentModel model)
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

    public async Task<(bool, string)> ChangeAppointmentStatus(string userId, string appointmentId, Status status)
    {
        try
        {
            var appointment = await this.repo
                .All<Appointment>()
                .Where(a => a.UserId == userId && a.Id.ToString() == appointmentId)
                .SingleOrDefaultAsync();
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
    public async Task<(bool, string)> CancelAppointment(string userId, string appointmentId)
    {
        return await ChangeAppointmentStatus(userId, appointmentId, Status.Cancelled);
    }
    public async Task<IEnumerable<CalendarAppointmentViewModel>> GetAllAppointmentsByMonth(int month)
    {
        Guard.AgainstOutOfRange(month, 1, 12, nameof(month));
        var appointments = await this.repo.All<Appointment>()
            .Where(a => a.Status != Status.Cancelled && a.Start.Month == month)
            .Select(a => new CalendarAppointmentViewModel { Start = a.Start })
            .OrderBy(a => a.Start.Day)
            .ToListAsync();

        return appointments;
    }
    public async Task<IEnumerable<UserAppointmentViewModel>> GetUserAppointments(string userId)
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
    public async Task<IEnumerable<AdminPanelAppointmentViewModel>> GetAllPendingAppointments()
    {
        var appointments = await this.repo.All<Appointment>()
            .Where(a => a.Status == Status.Pending)
            .Select(a => new AdminPanelAppointmentViewModel
            {
                Start = a.Start,
                UserId = a.UserId,
                Status = a.Status
            })
            .OrderByDescending(a => a.Start)
            .ThenByDescending(a => a.Start)
            .ToListAsync();

        return appointments;
    }

    public async Task<CalendarAppointmentViewModel> GetPreviousFreeAppointment(DateTime date, int workStart, int workEnd)
    {

        while (await CheckIfAppointmentExists(date))
        {
            date = date.AddMinutes(-30);
            if (date.Hour >= workStart) continue;
            date = date.AddDays(-1);
            date = date.AddHours(9);
        }

        if (date < DateTime.Now) return null;
        return new CalendarAppointmentViewModel { Start = date };
    }
    public async Task<CalendarAppointmentViewModel> GetNextFreeAppointment(DateTime date, int workStart, int workEnd)
    {
        if (date.Minute is not (30 or 0) || date < DateTime.Now)
        {
            var now = DateTime.Now;
            date = new DateTime(now.Year, now.Month, now.Day, now.Hour, 30,0);
            if (date < now)
            {
                date = date.AddMinutes(30);
            }

            if (date.Hour >= workEnd)
            {
                date = date.AddDays(1);
                date = date.AddHours((date.Hour - workStart) * -1);
            }
        }

        while (await CheckIfAppointmentExists(date))
        {
            date = date.AddMinutes(30);
            if (date.Hour < workEnd) continue;
            date = date.AddDays(1);
            date = date.AddHours((date.Hour - workStart) * -1);
        }

        return new CalendarAppointmentViewModel { Start = date };
    }

    public async Task<bool> CheckIfAppointmentExists(DateTime date)
    {
        var test = await this.repo.All<Appointment>()
            .Where(a => a.Status != Status.Cancelled)
            .ToListAsync();
        test = test.Where(a => a.Start == date).ToList();
        var exists = await this.repo.All<Appointment>()
            .Where(a => a.Start == date && a.Status != Status.Cancelled)
            .FirstOrDefaultAsync() != null;
        return exists;
    }
}
