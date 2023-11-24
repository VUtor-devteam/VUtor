using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
