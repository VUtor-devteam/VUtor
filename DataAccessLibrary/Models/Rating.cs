using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
