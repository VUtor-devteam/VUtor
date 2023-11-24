namespace DataAccessLibrary.Models;

public class Announcement
{
    public Guid ID { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; }
    public string AuthorName { get; set; }
    public string? Href { get; set; }
    public string? uploadDate { get; set; }
}
