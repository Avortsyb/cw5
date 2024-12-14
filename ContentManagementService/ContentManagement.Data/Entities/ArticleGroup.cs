namespace ContentManagement.Data.Entities;

public class ArticleGroup {
    public int ArticleId { get; set; }
    public int GroupId { get; set; }
    
    public Article Article { get; set; }
}