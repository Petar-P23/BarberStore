using BarberStore.Core.Models.Articles;

namespace BarberStore.Core.Contracts;

public interface IArticleService
{
    public IQueryable<ArticleViewModel> GetAllArticlesAsQueryable();
    public Task<IEnumerable<ArticleViewModel>> GetAllArticles();
    public Task<IEnumerable<ArticleViewModel>> GetAllArticles(int count);
    public Task<IEnumerable<ArticleViewModel>> GetAllArticles(int count, int page);
    public Task<IEnumerable<ArticleViewModel>> GetTopArticles();
    public Task<(bool, string)> CreateArticle(string userId, ArticleModel? article);
}