using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Migrations;
using DataAccessLibrary.FileRepo.Model;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLibrary.FileRepo
{
    public class FileRepository : GenericRepository<UserFile>, IFileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainer;
        public FileRepository(ApplicationDbContext context, BlobServiceClient blobServiceClient) : base(context)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
            _blobContainer = _blobServiceClient.GetBlobContainerClient("user-resources");
        }
        public async Task<UserFile> UploadFileAsync(IBrowserFile blob, string fileName, string title, string? description, List<int> topicsId, int folderId, string userId)
        {
            string blobName = GenerateBlobName(fileName);
            var blobClient = _blobContainer.GetBlobClient(blobName);

            await blobClient.UploadAsync(blob.OpenReadStream(), overwrite: true);
            var blobUrl = blobClient.Uri.AbsoluteUri;
            var topics = _context.Topics.Where(x => topicsId.Contains(x.Id)).ToList();

            var file = new UserFile
            {
                Title = title,
                Description = description,
                FileName = blobName,
                BlobUri = blobUrl,
                Topics = topics,
                FolderId = folderId,
                ProfileId = userId,
                CreationDate = new profileCreationDate()
            };

            _context.Folders.Where(x => x.Id == folderId).First().Files.Add(file);
            _context.Profiles.Where(x => x.Id == userId).First().UserItems.Add(file);
            foreach ( var topic in topics)
            {
                _context.Topics.Where(x => x.Id == topic.Id).First().UserFiles.Add(file);
            }
            _context.UserFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
    }

        public Task<List<UserFile>> GetFilesForUserAsync(string userId)
        {
            return _context.UserFiles.Include(e => e.ProfileId).Where(e => e.ProfileId == userId).ToListAsync();
        }
        public Task<UserFile> GetFilesForTitleAsync(string title)
        {
            try
            {
                return _context.UserFiles.Where(e => e.Title.Contains(title)).FirstAsync();
            }
            catch
            {
                return null;
            }
        }

        public Task<UserFile> GetFileAsync(int fileId)
        {
            try
            {
                return _context.UserFiles.FirstAsync(e => e.Id == fileId);
            }
            catch
            {
                return null;
            }
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

        public async Task<BlobDto> DownloadAsync(int fileId)
        {
            var blobFile = _context.UserFiles.FindAsync(fileId).Result;
            if (blobFile != null)
            {
                var blobFileName = blobFile.FileName;
                BlobClient file = _blobContainer.GetBlobClient(blobFileName);

                var data = await file.OpenReadAsync();

                var content = await file.DownloadContentAsync();
                var contentType = content.Value.Details.ContentType;

                return new BlobDto { Content = data, Name = blobFileName, ContentType = contentType };
            }
            return null;
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var file = _context.UserFiles.FindAsync(fileId).Result;
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
                .Include(e => e.Topics)
                .Where(f => f.FolderId == folderId)
                .ToListAsync();
        }

        public Task<List<UserFile>> GetFilesForTopicAsync(int topicId)
        {
            var topic = _context.Topics.FindAsync(topicId).Result;
            return _context.UserFiles
                .Where(f => f.Topics.Contains(topic))
                .ToListAsync();
        }

        private string GenerateBlobName(string fileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        }

    }
}