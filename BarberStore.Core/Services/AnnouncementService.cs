using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Announcements;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Core.Services;

public class AnnouncementService : DataService, IAnnouncementService
{
    public AnnouncementService(IApplicationDbRepository repo) 
        : base(repo)
    {
    }

    public async Task<(bool, string)> CreateAnnouncement(string userId, AnnouncementModel? Announcement)
    {
        try
        {
            var announcementEntity = new Announcement
            {
                Title = Announcement.Title,
                MainText = Announcement.MainText,
                ImagePath = Announcement.ImagePath,
                PublishDate = Announcement.PublishDate.Value,
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