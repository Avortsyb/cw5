using ContentManagement.Data.Entities;
using ContentManagement.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace ContentManagement.Application.Services;

public class ArticleService(IArticleRepository articleRepository, ILogger<ArticleService> logger) : IArticleService {
    public async Task<IEnumerable<Article>> GetAllAsync() {
        try {
            logger.LogInformation("Fetching all articles...");
            return await articleRepository.GetAllAsync();
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while fetching all articles.");
            throw new ApplicationException("An error occurred while fetching articles. Please try again later.", ex);
        }
    }

    public async Task<Article?> GetByIdAsync(int articleId) {
        try {
            logger.LogInformation("Fetching article by ID: {ArticleId}", articleId);
            var article = await articleRepository.GetByIdAsync(articleId);

            if (article == null) {
                logger.LogWarning("Article with ID {ArticleId} not found.", articleId);
            }

            return article;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while fetching article by ID: {ArticleId}", articleId);
            throw new ApplicationException($"An error occurred while fetching the article with ID {articleId}.", ex);
        }
    }

    public async Task AddAsync(Article article) {
        try {
            logger.LogInformation("Adding a new article...");
            await articleRepository.AddAsync(article);
            logger.LogInformation("Article added successfully. ID: {ArticleId}", article.ArticleId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while adding a new article.");
            throw new ApplicationException("An error occurred while adding the article.", ex);
        }
    }

    public async Task UpdateAsync(Article updatedArticle) {
        try {
            logger.LogInformation("Updating article with ID: {ArticleId}", updatedArticle.ArticleId);

            var existingArticle = await articleRepository.GetByIdAsync(updatedArticle.ArticleId);
            if (existingArticle == null) {
                throw new KeyNotFoundException($"Article with ID {updatedArticle.ArticleId} not found.");
            }

            // Merge existing values with updated values
            existingArticle.Title = string.IsNullOrWhiteSpace(updatedArticle.Title) 
                ? existingArticle.Title 
                : updatedArticle.Title;

            existingArticle.Content = string.IsNullOrWhiteSpace(updatedArticle.Content) 
                ? existingArticle.Content 
                : updatedArticle.Content;

            existingArticle.IsPublic = updatedArticle.IsPublic;

            existingArticle.LastEditDate = DateTime.UtcNow;

            await articleRepository.UpdateAsync(existingArticle);
            logger.LogInformation("Article updated successfully. ID: {ArticleId}", updatedArticle.ArticleId);
        } catch (KeyNotFoundException ex) {
            logger.LogWarning(ex.Message);
            throw;
        } catch (Exception ex) {
            logger.LogError(ex, "Error occurred while updating article with ID: {ArticleId}", updatedArticle.ArticleId);
            throw new ApplicationException($"An error occurred while updating the article with ID {updatedArticle.ArticleId}.", ex);
        }
    }


    public async Task DeleteAsync(int articleId) {
        try {
            logger.LogInformation("Deleting article with ID: {ArticleId}", articleId);
            var article = await articleRepository.GetByIdAsync(articleId);
            if (article == null) {
                logger.LogWarning("Article with ID {ArticleId} not found. Nothing to delete.", articleId);
                return;
            }

            await articleRepository.DeleteAsync(article);
            logger.LogInformation("Article deleted successfully. ID: {ArticleId}", articleId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while deleting article with ID: {ArticleId}", articleId);
            throw new ApplicationException($"An error occurred while deleting the article with ID {articleId}.", ex);
        }
    }

    public async Task<IEnumerable<Article>> GetPublicArticlesAsync() {
        try {
            logger.LogInformation("Fetching public articles...");
            return await articleRepository.GetPublicArticlesAsync();
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while fetching public articles.");
            throw new ApplicationException("An error occurred while fetching public articles. Please try again later.",
                ex);
        }
    }

    public async Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(int authorId) {
        try {
            logger.LogInformation("Fetching articles by author ID: {AuthorId}", authorId);
            return await articleRepository.GetArticlesByAuthorIdAsync(authorId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while fetching articles by author ID: {AuthorId}", authorId);
            throw new ApplicationException(
                $"An error occurred while fetching articles for author ID {authorId}. Please try again later.", ex);
        }
    }

    public async Task<IEnumerable<Article>> GetArticlesByGroupIdAsync(int groupId) {
        try {
            logger.LogInformation("Fetching articles for group ID: {GroupId}", groupId);
            return await articleRepository.GetArticlesByGroupIdAsync(groupId);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while fetching articles for group ID: {GroupId}", groupId);
            throw new ApplicationException($"An error occurred while fetching articles for group ID {groupId}.", ex);
        }
    }

    public async Task AddGroupToArticleAsync(int articleId, int groupId) {
        try {
            logger.LogInformation("Adding group ID {GroupId} to article ID {ArticleId}", groupId, articleId);

            var article = await articleRepository.GetByIdAsync(articleId);
            if (article == null) {
                logger.LogWarning("Article with ID {ArticleId} not found. Cannot add group.", articleId);
                throw new KeyNotFoundException($"Article with ID {articleId} not found.");
            }

            var existingLink = await articleRepository.IsGroupLinkedToArticleAsync(articleId, groupId);
            if (existingLink) {
                logger.LogWarning("Group ID {GroupId} is already linked to article ID {ArticleId}.", groupId,
                    articleId);
                throw new InvalidOperationException($"Group ID {groupId} is already linked to article ID {articleId}.");
            }

            await articleRepository.AddGroupToArticleAsync(articleId, groupId);
            logger.LogInformation("Group ID {GroupId} added to article ID {ArticleId} successfully.", groupId,
                articleId);
        }
        catch (KeyNotFoundException) {
            throw;
        }
        catch (InvalidOperationException) {
            throw;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while adding group ID {GroupId} to article ID {ArticleId}.", groupId,
                articleId);
            throw new ApplicationException($"An error occurred while adding group to article ID {articleId}.", ex);
        }
    }

    public async Task RemoveGroupFromArticleAsync(int articleId, int groupId) {
        try {
            logger.LogInformation("Removing group ID {GroupId} from article ID {ArticleId}", groupId, articleId);

            var article = await articleRepository.GetByIdAsync(articleId);
            if (article == null) {
                logger.LogWarning("Article with ID {ArticleId} not found. Cannot remove group.", articleId);
                throw new KeyNotFoundException($"Article with ID {articleId} not found.");
            }

            var existingLink = await articleRepository.IsGroupLinkedToArticleAsync(articleId, groupId);
            if (!existingLink) {
                logger.LogWarning("Group ID {GroupId} is not linked to article ID {ArticleId}. Nothing to remove.",
                    groupId, articleId);
                throw new InvalidOperationException($"Group ID {groupId} is not linked to article ID {articleId}.");
            }

            await articleRepository.RemoveGroupFromArticleAsync(articleId, groupId);
            logger.LogInformation("Group ID {GroupId} removed from article ID {ArticleId} successfully.", groupId,
                articleId);
        }
        catch (KeyNotFoundException) {
            throw;
        }
        catch (InvalidOperationException) {
            throw;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while removing group ID {GroupId} from article ID {ArticleId}.",
                groupId, articleId);
            throw new ApplicationException($"An error occurred while removing group from article ID {articleId}.", ex);
        }
    }
    
    public async Task<IEnumerable<int>> GetGroupsByArticleIdAsync(int articleId) {
        try {
            logger.LogInformation("Fetching groups for article ID: {ArticleId}", articleId);
            var article = await articleRepository.GetByIdAsync(articleId);
            if (article == null) {
                logger.LogWarning("Article with ID {ArticleId} not found.", articleId);
                throw new KeyNotFoundException($"Article with ID {articleId} not found.");
            }

            return await articleRepository.GetGroupsByArticleIdAsync(articleId);
        } catch (KeyNotFoundException) {
            throw;
        } catch (Exception ex) {
            logger.LogError(ex, "Error occurred while fetching groups for article ID: {ArticleId}", articleId);
            throw new ApplicationException($"An error occurred while fetching groups for article ID {articleId}.", ex);
        }
    }

}