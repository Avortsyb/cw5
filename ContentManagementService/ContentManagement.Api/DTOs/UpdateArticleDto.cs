namespace ContentManagement.Api.DTOs;

public class UpdateArticleDto {
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool? IsPublic { get; set; }
}