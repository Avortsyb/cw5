using ContentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContentManagement.Data.Repositories;

public class ArticleRepository(ApplicationDbContext context) : IArticleRepository {
    public async Task<IEnumerable<Article>> GetAllAsync() {
        return await context.Articles.ToListAsync();
    }

    public async Task<Article?> GetByIdAsync(int articleId) {
        return await context.Articles.FindAsync(articleId);
    }

    public async Task AddAsync(Article article) {
        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Article article) {
        context.Articles.Update(article);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Article article) {
        context.Articles.Remove(article);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Article>> GetPublicArticlesAsync() {
        return await context.Articles
            .Where(a => a.IsPublic)
            .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(int authorId) {
        return await context.Articles
            .Where(a => a.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetArticlesByGroupIdAsync(int groupId) {
        return await context.Articles
            .Where(a => context.ArticleGroups
                .Any(ag => ag.ArticleId == a.ArticleId && ag.GroupId == groupId))
            .ToListAsync();
    }

    public async Task AddGroupToArticleAsync(int articleId, int groupId) {
        var articleGroup = new ArticleGroup { ArticleId = articleId, GroupId = groupId };
        await context.ArticleGroups.AddAsync(articleGroup);
        await context.SaveChangesAsync();
    }

    public async Task RemoveGroupFromArticleAsync(int articleId, int groupId) {
        var articleGroup = await context.ArticleGroups
            .FirstOrDefaultAsync(ag => ag.ArticleId == articleId && ag.GroupId == groupId);

        if (articleGroup != null) {
            context.ArticleGroups.Remove(articleGroup);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsGroupLinkedToArticleAsync(int articleId, int groupId) {
        return await context.ArticleGroups.AnyAsync(ag => ag.ArticleId == articleId && ag.GroupId == groupId);
    }

    public async Task<IEnumerable<int>> GetGroupsByArticleIdAsync(int articleId) {
        return await context.ArticleGroups
            .Where(ag => ag.ArticleId == articleId)
            .Select(ag => ag.GroupId)
            .ToListAsync();
    }

    public async Task RemoveGroupLinksAsync(int groupId)
    {
        var linksToRemove = context.ArticleGroups.Where(ag => ag.GroupId == groupId);

        if (linksToRemove.Any())
        {
            context.ArticleGroups.RemoveRange(linksToRemove);
            await context.SaveChangesAsync();
        }
    }
}