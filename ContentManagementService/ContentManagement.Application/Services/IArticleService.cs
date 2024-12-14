using ContentManagement.Data.Entities;

namespace ContentManagement.Application.Services;

public interface IArticleService {
    Task<IEnumerable<Article>> GetAllAsync();
    Task<Article?> GetByIdAsync(int articleId);
    Task AddAsync(Article article);
    Task UpdateAsync(Article article);
    Task DeleteAsync(int articleId);
    Task<IEnumerable<Article>> GetPublicArticlesAsync();
    Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(int authorId);
    Task<IEnumerable<Article>> GetArticlesByGroupIdAsync(int groupId);
    Task AddGroupToArticleAsync(int articleId, int groupId);
    Task RemoveGroupFromArticleAsync(int articleId, int groupId);
    Task<IEnumerable<int>> GetGroupsByArticleIdAsync(int articleId);
}