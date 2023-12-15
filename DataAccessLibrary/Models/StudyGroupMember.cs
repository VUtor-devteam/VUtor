using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class StudyGroupMember
    {
        [Key]
        public int Id { get; set; }
        public int StudyGroupId { get; set; }
        public string ParticipantId { get; set; }
    }
}
