﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class UserFile
    {
        [Key]
        public int Id { get; set; }
        public int TopicId { get; set; }
        public TopicEntity Topic { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string FileName { get; set; }
    }
}
