using ContentManagement.Api.DTOs;
using ContentManagement.Application.Services;
using ContentManagement.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase {
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService) {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllArticles() {
        try {
            var articles = await _articleService.GetAllAsync();
            var articleDtos = articles.Select(a => new ArticleDto {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                IsPublic = a.IsPublic,
                AuthorId = a.AuthorId,
                PublishDate = a.PublishDate,
                LastEditDate = a.LastEditDate
            });
            return Ok(articleDtos);
        }
        catch (Exception ex) {
            return StatusCode(500, $"An error occurred while fetching articles: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticleById(int id) {
        try {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) {
                return NotFound("Article not found.");
            }

            var articleDto = new ArticleDto {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                IsPublic = article.IsPublic,
                AuthorId = article.AuthorId,
                PublishDate = article.PublishDate,
                LastEditDate = article.LastEditDate
            };

            return Ok(articleDto);
        }
        catch (Exception ex) {
            return StatusCode(500, $"An error occurred while fetching the article: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddArticle([FromBody] CreateArticleDto createDto) {
        try {
            if (createDto == null) {
                return BadRequest("Invalid article data.");
            }

            var article = new Article {
                Title = createDto.Title,
                Content = createDto.Content,
                IsPublic = createDto.IsPublic,
                AuthorId = createDto.AuthorId,
                PublishDate = DateTime.UtcNow,
                LastEditDate = DateTime.UtcNow
            };

            await _articleService.AddAsync(article);

            var articleDto = new ArticleDto {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                IsPublic = article.IsPublic,
                AuthorId = article.AuthorId,
                PublishDate = article.PublishDate,
                LastEditDate = article.LastEditDate
            };

            return CreatedAtAction(nameof(GetArticleById), new { id = article.ArticleId }, articleDto);
        }
        catch (Exception ex) {
            return StatusCode(500, $"An error occurred while adding the article: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleDto updateDto) {
        try {
            if (updateDto == null) {
                return BadRequest("Invalid article data.");
            }

            var existingArticle = await _articleService.GetByIdAsync(id);
            if (existingArticle == null) {
                return NotFound("Article not found.");
            }

            var updatedArticle = new Article {
                ArticleId = id,
                Title = string.IsNullOrWhiteSpace(updateDto.Title) ? existingArticle.Title : updateDto.Title,
                Content = string.IsNullOrWhiteSpace(updateDto.Content) ? existingArticle.Content : updateDto.Content,
                IsPublic = updateDto.IsPublic ?? existingArticle.IsPublic
            };

            await _articleService.UpdateAsync(updatedArticle);
            return NoContent();
        } catch (KeyNotFoundException ex) {
            return NotFound(ex.Message);
        } catch (Exception ex) {
            return StatusCode(500, $"An error occurred while updating the article: {ex.Message}");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id) {
        try {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) {
                return NotFound("Article not found.");
            }

            await _articleService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) {
            return StatusCode(500, $"An error occurred while deleting the article: {ex.Message}");
        }
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublicArticles() {
        try {
            var articles = await _articleService.GetPublicArticlesAsync();
            var articleDtos = articles.Select(a => new ArticleDto {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                IsPublic = a.IsPublic,
                AuthorId = a.AuthorId,
                PublishDate = a.PublishDate,
                LastEditDate = a.LastEditDate
            });
            return Ok(articleDtos);
        }
        catch (Exception ex) {
            return StatusCode(500, $"An error occurred while fetching public articles: {ex.Message}");
        }
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetArticlesByGroupId(int groupId) {
        try {
            var articles = await _articleService.GetArticlesByGroupIdAsync(groupId);
            var articleDtos = articles.Select(a => new ArticleDto {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                IsPublic = a.IsPublic,
                AuthorId = a.AuthorId,
                PublishDate = a.PublishDate,
                LastEditDate = a.LastEditDate
            });
            return Ok(articleDtos);
        }
        catch (Exception ex) {
            return StatusCode(500, $"An error occurred while fetching articles for the group: {ex.Message}");
        }
    }
    
    [HttpGet("author/{authorId}")]
    public async Task<IActionResult> GetArticlesByAuthorId(int authorId) {
        try {
            var articles = await _articleService.GetArticlesByAuthorIdAsync(authorId);
            var articleDtos = articles.Select(a => new ArticleDto {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                IsPublic = a.IsPublic,
                AuthorId = a.AuthorId,
                PublishDate = a.PublishDate,
                LastEditDate = a.LastEditDate
            });
            return Ok(articleDtos);
        }
        catch (Exception ex) {
            return StatusCode(500, $"An error occurred while fetching articles for the author: {ex.Message}");
        }
    }
    
    // [HttpPost("{articleId}/group/{groupId}")]
    // public async Task<IActionResult> AddGroupToArticle(int articleId, int groupId) {
    //     try {
    //         await _articleService.AddGroupToArticleAsync(articleId, groupId);
    //         return Ok($"Group ID {groupId} added to article ID {articleId} successfully.");
    //     } catch (KeyNotFoundException ex) {
    //         return NotFound(ex.Message);
    //     } catch (InvalidOperationException ex) {
    //         return BadRequest(ex.Message);
    //     } catch (Exception ex) {
    //         return StatusCode(500, $"An error occurred while adding group to article: {ex.Message}");
    //     }
    // }
    
    [HttpPost("{articleId}/groups")]
    public async Task<IActionResult> AddGroupToArticle(int articleId, [FromBody] int groupId, [FromServices] GroupValidationService validationService)
    {
        try
        {
            var isValid = await validationService.ValidateGroupAsync(groupId);

            if (!isValid)
            {
                return BadRequest("Group does not exist.");
            }

            await _articleService.AddGroupToArticleAsync(articleId, groupId);

            return Ok("Group added to article successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }


    [HttpDelete("{articleId}/group/{groupId}")]
    public async Task<IActionResult> RemoveGroupFromArticle(int articleId, int groupId) {
        try {
            await _articleService.RemoveGroupFromArticleAsync(articleId, groupId);
            return Ok($"Group ID {groupId} removed from article ID {articleId} successfully.");
        } catch (KeyNotFoundException ex) {
            return NotFound(ex.Message);
        } catch (InvalidOperationException ex) {
            return BadRequest(ex.Message);
        } catch (Exception ex) {
            return StatusCode(500, $"An error occurred while removing group from article: {ex.Message}");
        }
    }
    
    [HttpGet("{articleId}/groups")]
    public async Task<IActionResult> GetGroupsByArticleId(int articleId) {
        try {
            var groups = await _articleService.GetGroupsByArticleIdAsync(articleId);
            return Ok(groups);
        } catch (KeyNotFoundException ex) {
            return NotFound(ex.Message);
        } catch (Exception ex) {
            return StatusCode(500, $"An error occurred while fetching groups for article ID {articleId}: {ex.Message}");
        }
    }
}