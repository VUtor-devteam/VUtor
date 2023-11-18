using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class UserFile : UserItem
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
        public List<TopicEntity> Topics { get; set; }
        public string FileName { get; set; }
        public string BlobUri { get; set; }
    }
}
