using BarberStore.Infrastructure.Data.Repositories;

namespace BarberStore.Core.Services;

public abstract class DataService
{
    protected readonly IApplicationDbRepository repo;

    protected DataService(IApplicationDbRepository repo)
    {
        this.repo = repo;
    }
}