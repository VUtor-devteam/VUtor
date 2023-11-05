using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.FileRepo
{
    public interface IFolderRepository
    {
        Task<Folder> CreateFolder(Folder folder);
        Task<Folder> GetFolder(int id);
        Task<List<Folder>> GetFolders();
        Task UpdateFolder(Folder folder);
        Task DeleteFolder(int id);
        Task MoveFolder(int folderId, int newParentId);
    }
}
