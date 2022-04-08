using BarberStore.Core.Models.Announcements;

namespace BarberStore.Core.Contracts;

public interface IAnnouncementService
{
    public Task<(bool, string)> CreateAnnouncement(string userId, AnnouncementModel? announcement);
    public IQueryable<AnnouncementViewModel> GetAllAnnouncementsAsQueryable();
    public Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncements();
    public Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncements(int count);
    public Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncements(int count, int page);
    public Task<IEnumerable<AnnouncementViewModel>> GetTopAnnouncements();
}