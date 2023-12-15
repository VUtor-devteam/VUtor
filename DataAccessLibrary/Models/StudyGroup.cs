using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class StudyGroup
    {
        [Key]
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public DateTime GroupDate { get; set; }
        public bool GroupPlace { get; set; }
        public string Subject { get; set; }
    }
}
