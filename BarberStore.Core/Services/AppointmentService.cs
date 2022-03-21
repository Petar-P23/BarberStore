using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Enums;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Core.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IApplicationDbRepository repo;

    public AppointmentService(IApplicationDbRepository repo)
    {
        this.repo = repo;
    }

    public async Task<bool> CreateAppointment(AppointmentModel model)
    {
        try
        {
            var services = await repo
                .All<Service>()
                .Where(s => model.Services.Contains(s.Id.ToString()))
                .ToListAsync();

            var appointment = new Appointment
            {
                Start = model.Start,
                UserId = model.UserId,
                ServicesToDo = services
            };

            await repo.AddAsync(appointment);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    public async Task<bool> CancelAppointment(string appointmentId)
    {
        try
        {
            var appointment = await repo.All<Appointment>()
                .Where(a => a.Id.ToString() == appointmentId)
                .SingleOrDefaultAsync();

            appointment.Status = Status.Cancelled;
            await repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    public async Task<IEnumerable<CalendarAppointmentViewModel>> GetAllCalendarAppointments(int month)
    {
        var appointments = await repo.All<Appointment>()
            .Where(a => a.Status != Status.Cancelled && a.Start.Month == month)
            .Select(a => new CalendarAppointmentViewModel { Start = a.Start })
            .ToListAsync();

        return appointments;
    }
    public async Task<IEnumerable<UserAppointmentViewModel>> GetUserAppointments(string userId)
    {
        var appointments = await repo.All<Appointment>()
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
}