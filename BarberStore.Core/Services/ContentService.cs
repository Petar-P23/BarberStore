using BarberStore.Core.Contracts;
using BarberStore.Infrastructure.Data.Repositories;

namespace BarberStore.Core.Services;

public class ContentService : DataService, IContentService
{
    public ContentService(IApplicationDbRepository repo)
        : base(repo)
    {
    }


}