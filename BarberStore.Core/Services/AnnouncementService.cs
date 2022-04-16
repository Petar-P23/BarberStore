using BarberStore.Core.Common;
using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Announcements;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using static BarberStore.Core.Constants.ExceptionMessageConstants;

namespace BarberStore.Core.Services;

public class AnnouncementService : DataService, IAnnouncementService
{
    public AnnouncementService(IApplicationDbRepository repo)
        : base(repo)
    {
    }

    public async Task<(bool, string)> CreateAnnouncementAsync(string mainText)
    {
        try
        {
            Guard.AgainstNull(mainText);
            var announcementEntity = new Announcement
            {
                MainText = mainText,
                PublishDate = DateTime.Now
            };

            await this.repo.AddAsync(announcementEntity);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return (false, UnexpectedErrorMessage);
        }

        return (true, string.Empty);
    }
    public IQueryable<AnnouncementViewModel> GetAllAnnouncementsAsQueryable()
    {
        return this.repo.All<Announcement>()
            .Select(a => new AnnouncementViewModel
            {
                Id = a.Id.ToString(),
                MainText = a.MainText,
                PublishDate = a.PublishDate,
            })
            .OrderByDescending(a => a.PublishDate);
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncementsAsync()
    {
        return await this.GetAllAnnouncementsAsQueryable().ToListAsync();
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncementsAsync(int count)
    {
        return await this.GetAllAnnouncementsAsQueryable()
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncementsAsync(int count, int page)
    {
        return await this.GetAllAnnouncementsAsQueryable()
            .Skip(page * count)
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetTopAnnouncementsAsync()
    {
        return await this.GetAllAnnouncementsAsync(10);
    }

    public async Task<bool> RemoveAnnouncementAsync(string id)
    {
        try
        {
            await this.repo.DeleteAsync<Announcement>(Guid.Parse(id));
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}