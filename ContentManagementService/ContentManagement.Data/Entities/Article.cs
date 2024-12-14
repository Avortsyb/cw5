namespace ContentManagement.Data.Entities;

public class Article {
    public int ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsPublic { get; set; }
    public int AuthorId { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime LastEditDate { get; set; }
    
    public ICollection<ArticleGroup> ArticleGroups { get; set; } = new List<ArticleGroup>();

}