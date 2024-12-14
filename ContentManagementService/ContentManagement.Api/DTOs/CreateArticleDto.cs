namespace ContentManagement.Api.DTOs;

public class CreateArticleDto {
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsPublic { get; set; }
    public int AuthorId { get; set; }
}