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

    public async Task<(bool, string)> CreateAnnouncement(string userId, AnnouncementModel? announcement)
    {
        try
        {
            Guard.AgainstNull(announcement);
            var announcementEntity = new Announcement
            {
                Title = announcement.Title,
                MainText = announcement.MainText,
                ImagePath = announcement.ImagePath,
                PublishDate = announcement.PublishDate.Value,
                PublishUserId = userId
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
                Title = a.Title,
                MainText = a.MainText,
                ImagePath = a.ImagePath,
                PublishDate = a.PublishDate,
                Publisher = $"{a.PublishUser.FirstName} {a.PublishUser.LastName}"
            })
            .OrderByDescending(a => a.PublishDate);
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncements()
    {
        return await this.GetAllAnnouncementsAsQueryable().ToListAsync();
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncements(int count)
    {
        return await this.GetAllAnnouncementsAsQueryable()
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncements(int count, int page)
    {
        return await this.GetAllAnnouncementsAsQueryable()
            .Skip(page * count)
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<AnnouncementViewModel>> GetTopAnnouncements()
    {
        return await this.GetAllAnnouncements(10);
    }
}