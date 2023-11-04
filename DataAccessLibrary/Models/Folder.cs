﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<UserFile> Files { get; set; } = new List<UserFile>();
        public List<Folder>? SubFolders { get; set; } = new List<Folder>();        
    }
}
