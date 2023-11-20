using DataAccessLibrary.FileRepo.Model;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace DataAccessLibrary.FileRepo
{
    public interface IFileRepository
    {
        Task<UserFile> UploadFileAsync(IBrowserFile blob, string fileName, string title, string? description, List<int> topicsId, int folderId, string userId);
        Task EditFile(UserFile file, string? title, string? description, List<int>? topics, int? folderId);
        Task<List<UserFile>> GetFilesForTopicAsync(int topicId);
        Task<List<UserFile>> GetFilesForBlobUrls(List<string> blobUrls);
        Task<List<UserFile>> GetFilesForUserAsync(string userId);
        Task<List<UserFile>> GetFilesForFolderAsync(int folderId);
        Task<UserFile> GetFileAsync(int fileId);
        Task DeleteFileAsync(int fileId);
    }
}
