using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models;

public class UserItem
{
    [Key]
    public int Id { get; set; }
    public string ProfileId { get; set; }
    public ProfileEntity Profile { get; set; }
    public profileCreationDate CreationDate { get; set; }
}
