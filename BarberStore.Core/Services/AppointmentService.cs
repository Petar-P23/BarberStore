using BarberStore.Core.Common;
using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using static BarberStore.Core.Constants.ExceptionMessageConstants;

namespace BarberStore.Core.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IApplicationDbRepository repo;

    public AppointmentService(IApplicationDbRepository repo)
    {
        this.repo = repo;
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

            var services = await this.repo
                .All<Service>()
                .Where(s => model.Services.Contains(s.Id.ToString()))
                .ToListAsync();

            var appointment = new Appointment
            {
                Start = model.Start,
                UserId = model.UserId,
                ServicesToDo = services
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
    public async Task<(bool, string)> EditAppointment(string userId, string appointmentId, AppointmentModel model)
    {
        try
        {
            Guard.AgainstNull(model);
            if (model.Start.Day < DateTime.Today.Day)
            {
                throw new ArgumentException(AppointmentInvalidDateException);
            }

            var services = await this.repo
                .All<Service>()
                .Where(s => model.Services.Contains(s.Id.ToString()))
                .ToListAsync();

            var appointment = await this.repo
                .All<Appointment>()
                .Where(a => a.UserId == userId && a.Id.ToString() == appointmentId)
                .SingleOrDefaultAsync();
            Guard.AgainstNull(appointment);

            appointment.Start = model.Start;
            appointment.ServicesToDo = services;

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
        try
        {
            Guard.AgainstNullOrWhiteSpaceString(userId);
            Guard.AgainstNullOrWhiteSpaceString(appointmentId);

            var appointment = await this.repo.All<Appointment>()
                .Where(a => a.Id.ToString() == appointmentId && a.UserId == userId)
                .SingleOrDefaultAsync();
            Guard.AgainstNull(appointment);

            appointment.Status = Status.Cancelled;
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
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
                Start = a.Start,
                UserId = a.UserId,
                Services = a.ServicesToDo.Select(s => new ServiceModel
                {
                    Name = s.Name,
                    Price = s.Price,
                }).ToList(),
                Status = a.Status
            })
            .ToListAsync();

        return appointments;
    }
    public async Task<IEnumerable<AdminPanelAppointmentModel>> GetAllPendingAppointments()
    {
        var appointments = await this.repo.All<Appointment>()
            .Where(a => a.Status == Status.Pending)
            .Select(a => new AdminPanelAppointmentModel
            {
                Start = a.Start,
                UserId = a.UserId,
                Services = a.ServicesToDo.Select(s => new ServiceModel
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Price = s.Price,
                }).ToList(),
                Status = a.Status
            })
            .OrderBy(a => a.Start)
            .ToListAsync();

        return appointments;
    }
}