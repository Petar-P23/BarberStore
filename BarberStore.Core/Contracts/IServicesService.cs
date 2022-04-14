using BarberStore.Core.Models.Appointments;

namespace BarberStore.Core.Contracts;

public interface IServicesService
{
    public Task<IEnumerable<ServiceModel>> GetServicesAsync();
    ///// <summary>
    /////
    ///// </summary>
    ///// <returns>Returns a query of all articles sorted by date descending.</returns>
    //public IQueryable<ArticleViewModel> GetAllArticlesAsQueryable();
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns>Returns all articles sorted by date descending.</returns>
    //public Task<IEnumerable<ArticleViewModel>> GetAllArticles();
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="count">The number of articles to return.</param>
    ///// <returns>Returns articles sorted by date descending.</returns>
    //public Task<IEnumerable<ArticleViewModel>> GetAllArticles(int count);
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="count">The number of articles on a page.</param>
    ///// <param name="page">The page number.</param>
    ///// <returns>Returns a collection of articles, representing a page.</returns>
    //public Task<IEnumerable<ArticleViewModel>> GetAllArticles(int count, int page);
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns>Returns the latest 10 articles.</returns>
    //public Task<IEnumerable<ArticleViewModel>> GetTopArticles();
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="userId">The id of the user who created the article.</param>
    ///// <param name="article"></param>
    ///// <returns>Returns true if creation was successful, else an error is returned.</returns>
    //public Task<(bool, string)> CreateArticle(string userId, ArticleModel? article);
}