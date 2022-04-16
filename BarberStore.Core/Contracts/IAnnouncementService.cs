using BarberStore.Core.Models.Announcements;

namespace BarberStore.Core.Contracts;

public interface IAnnouncementService
{
    public Task<(bool, string)> CreateAnnouncementAsync(string mainText);
    public IQueryable<AnnouncementViewModel> GetAllAnnouncementsAsQueryable();
    public Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncementsAsync();
    public Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncementsAsync(int count);
    public Task<IEnumerable<AnnouncementViewModel>> GetAllAnnouncementsAsync(int count, int page);
    public Task<IEnumerable<AnnouncementViewModel>> GetTopAnnouncementsAsync();
    Task<bool> RemoveAnnouncementAsync(string id);
}