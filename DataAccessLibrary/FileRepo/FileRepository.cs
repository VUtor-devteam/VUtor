using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DataAccessLibrary.FileRepo
{
    public class FileRepository : GenericRepository<UserFile>, IFileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobContainerClient _blobContainer;
        public FileRepository(ApplicationDbContext context, BlobContainerClient blobContainerClient) : base(context)
        {
            _context = context;
            _blobContainer = blobContainerClient;
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName, string contentType, int folderId, string userId)
        {
            string blobName = GenerateBlobName(fileName);
            var blobClient = _blobContainer.GetBlobClient(blobName);

            BlobHttpHeaders headers = new BlobHttpHeaders();
            headers.ContentType = contentType;

            await blobClient.UploadAsync(fileStream, headers);

            var file = new UserFile
            {
                Title = fileName,
                FileName = fileName,
                BlobUri = blobClient.Uri.AbsoluteUri,
                FolderId = folderId,
                ProfileId = userId,
                CreationDate = new profileCreationDate()
            };

            _context.UserFiles.Add(file);
            _context.SaveChanges();

    }

        public Task<List<UserFile>> GetFilesForUserAsync(string userId)
        {
            return _context.UserFiles.Include(e => e.ProfileId).Where(e => e.ProfileId == userId).ToListAsync();
        }

        public Task<UserFile> GetFileAsync(int fileId)
        {
            return _context.UserFiles.FirstOrDefaultAsync(e => e.Id == fileId);
        }

        public Task MoveFileAsync(int fileId, int folderId)
        {
            var file = _context.UserFiles.FirstOrDefault(e => e.Id == fileId);
            if(file != null)
            {
                file.FolderId = folderId;
                 _context.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var file = _context.UserFiles.FirstOrDefault(e => e.Id == fileId);
            if (file != null)
            {
                var blob = new BlobClient(new Uri(file.BlobUri));
                await blob.DeleteAsync();

                var folder = _context.Folders
                    .Where(e => e.Id == file.FolderId)
                    .FirstOrDefault();
                if (folder != null)
                {
                    folder.Files.Remove(file);
                }
                var user = _context.Profiles
                    .Where(e => e.Id == file.ProfileId)
                    .FirstOrDefault();
                if (user != null)
                {
                    user.UserItems.Remove(file);
                }

                _context.UserFiles.Remove(file);
                _context.SaveChanges();
            }
        }
        public Task<List<UserFile>> GetFilesForFolderAsync(int folderId)
        {
            return _context.UserFiles
                .Where(f => f.FolderId == folderId)
                .ToListAsync();
        }

        public Task<List<UserFile>> GetFilesForTopicAsync(int topicId)
        {
            return _context.UserFiles
                .Where(f => f.TopicId == topicId)
                .ToListAsync();
        }

        private string GenerateBlobName(string fileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        }

    }
}
