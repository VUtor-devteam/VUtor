using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int MemberNumber { get; set; }

    }
}
