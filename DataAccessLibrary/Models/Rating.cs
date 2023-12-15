using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public string RecipientId { get; set; }
        public string ReviewerId { get; set; }
        public int Score { get; set; }
    }
}
