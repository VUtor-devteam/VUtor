using DataAccessLibrary.Models;

namespace DataAccessLibrary.FolderRepo
{
    public interface IFolderRepository
    {
        Task<Folder> CreateFolder(string name);
        Task<Folder> CreateSubFolder(string name, int parentFolderId);
        Task<Folder> GetFolder(int id);
        Task<List<Folder>> GetFolders();
        Task<List<Folder>> GetParentFolders();
        Task<List<Folder>> GetSubFolders(int id);
        Task DeleteFolder(int id);
    }
}
