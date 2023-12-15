using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int? ParentFolderId { get; set; }
        public Folder? ParentFolder { get; set; }
        public List<UserFile> Files { get; set; } = new List<UserFile>();
        public List<Folder> SubFolders { get; set; } = new List<Folder>();
    }
}
