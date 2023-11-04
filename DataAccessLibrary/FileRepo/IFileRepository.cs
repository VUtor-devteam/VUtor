using DataAccessLibrary.Models;

namespace DataAccessLibrary.FileRepo
{
    public interface IFileRepository
    {
        Task UploadFileAsync(Stream fileStream, string fileName, string contentType, int folderId, string userId);
        Task<List<UserFile>> GetFilesForTopicAsync(int topicId);
        Task<List<UserFile>> GetFilesForUserAsync(string userId);
        Task<List<UserFile>> GetFilesForFolderAsync(int folderId);
        Task<UserFile> GetFileAsync(int fileId);
        Task MoveFileAsync(int fileId, int folderId);
        Task DeleteFileAsync(int fileId);
    }
}
