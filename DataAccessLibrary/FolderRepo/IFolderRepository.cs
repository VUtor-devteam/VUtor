using DataAccessLibrary.Models;

namespace DataAccessLibrary.FolderRepo
{
    public interface IFolderRepository
    {
        Task<Folder> CreateFolder(string name);
        Task<Folder> CreateSubFolder(string name, int parentFolderId);
        // returns how many files added
        Task<int> AddFileToFolder(int folderId, UserFile file);
        Task<int> AddFilesToFolder(int folderId, List<UserFile> files);
        Task<int> AddSubFolder(int folderId, int subFolderId);
        Task<int> AddSubFolders(int folderId, List<int> subFolders);
        Task<Folder> GetFolder(int id);
        Task<List<Folder>> GetFolders();
        Task<List<Folder>> GetParentFolders();
        Task<List<Folder>> GetSubFolders(int id);
        Task DeleteFolder(int id);
        Task MoveFolder(int folderId, int newParentId);
    }
}
