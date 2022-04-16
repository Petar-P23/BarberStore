using BarberStore.Core.Common;
using BarberStore.Core.Contracts;
using BarberStore.Core.Models.Appointments;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Core.Services;

public class ServicesService : DataService, IServicesService
{
    public ServicesService(IApplicationDbRepository repo)
        : base(repo)
    {
    }

    public async Task<IEnumerable<ServiceModel>> GetServicesAsync()
    {
        return await this.repo.All<Service>()
            .OrderByDescending(s => s.Name)
            .ThenBy(s => s.Price)
            .Select(s => new ServiceModel
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Price = s.Price,
                Description = s.Description
            }).ToArrayAsync();
    }

    public async Task<bool> CreateServiceAsync(string name, string description, decimal price)
    {
        try
        {
            Guard.AgainstNullOrWhiteSpaceString(name);
            Guard.AgainstNullOrWhiteSpaceString(description);
            if (price <= 0) throw new ArgumentException("Price cannot be less than 0");

            var service = new Service
            {
                Name = name,
                Price = price,
                Description = description
            };

            await this.repo.AddAsync(service);
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveServiceAsync(string id)
    {
        try
        {
            Guard.AgainstNullOrWhiteSpaceString(id);

            await this.repo.DeleteAsync<Service>(Guid.Parse(id));
            await this.repo.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    //public async Task<(bool, string)> CreateArticle(string userId, ArticleModel? article)
    //{
    //    try
    //    {
    //        Guard.AgainstNull(article);
    //        var articleEntity = new Article
    //        {
    //            Title = article.Title,
    //            MainText = article.MainText,
    //            ImagePath = article.ImagePath,
    //            PublishDate = article.PublishDate.Value,
    //            PublishUserId = userId
    //        };

    //        await this.repo.AddAsync(articleEntity);
    //        await this.repo.SaveChangesAsync();
    //    }
    //    catch (Exception)
    //    {
    //        return (false, UnexpectedErrorMessage);
    //    }

    //    return (true, string.Empty);
    //}
    //public IQueryable<ArticleViewModel> GetAllArticlesAsQueryable()
    //{
    //    return this.repo.All<Article>()
    //        .Select(a => new ArticleViewModel
    //        {
    //            Id = a.Id.ToString(),
    //            Title = a.Title,
    //            MainText = a.MainText,
    //            ImagePath = a.ImagePath,
    //            PublishDate = a.PublishDate,
    //            Publisher = $"{a.PublishUser.FirstName} {a.PublishUser.LastName}"
    //        })
    //        .OrderByDescending(a => a.PublishDate);
    //}
    //public async Task<IEnumerable<ArticleViewModel>> GetAllArticles()
    //{
    //    return await this.GetAllArticlesAsQueryable().ToListAsync();
    //}
    //public async Task<IEnumerable<ArticleViewModel>> GetAllArticles(int count)
    //{
    //    return await this.GetAllArticlesAsQueryable()
    //        .Take(count)
    //        .ToListAsync();
    //}
    //public async Task<IEnumerable<ArticleViewModel>> GetAllArticles(int count, int page)
    //{
    //    return await this.GetAllArticlesAsQueryable()
    //        .Skip(page * count)
    //        .Take(count)
    //        .ToListAsync();
    //}
    //public async Task<IEnumerable<ArticleViewModel>> GetTopArticles()
    //{
    //    return await this.GetAllArticles(10);
    //}
}