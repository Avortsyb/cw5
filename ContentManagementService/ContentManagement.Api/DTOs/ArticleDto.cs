namespace ContentManagement.Api.DTOs;

public class ArticleDto {
    public int ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsPublic { get; set; }
    public int AuthorId { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime LastEditDate { get; set; }
}