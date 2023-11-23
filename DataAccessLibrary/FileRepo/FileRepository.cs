using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Migrations;
using DataAccessLibrary.FileRepo.Model;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace DataAccessLibrary.FileRepo
{
    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainer;
        public FileRepository(ApplicationDbContext context, BlobServiceClient blobServiceClient)
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

        public async Task EditFile(UserFile file, string title, string description, List<int> topics, int folderId)
        {           
            if(file != null)
            {
                var topicList = await _context.Topics.ToListAsync(); 
                if(!file.Title.Equals(title) && !title.IsNullOrEmpty())
                {
                    file.Title = title;
                }
                if(!description.IsNullOrEmpty() && !file.Description.Equals(description))
                {
                    file.Description = description;
                }
                if (!topicList.IsNullOrEmpty() && topicList.Count > 0)
                {
                    foreach (var topic in topics)
                    {
                        if (!file.Topics.Any(x => x.Id == topic))
                        {
                            var newToAdd = topicList.Where(x => x.Id == topic).First();
                            file.Topics.Add(newToAdd);
                            newToAdd.UserFiles.Add(file);
                        }
                    }
                    foreach (var oldTopic in file.Topics.ToList())
                    {
                        if (!topics.Contains(oldTopic.Id))
                        {
                            file.Topics.Remove(oldTopic);
                            oldTopic.UserFiles.Remove(file);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                if(folderId != null && !folderId.Equals(file.FolderId))
                {
                    var oldFolder = await _context.Folders.Where(x => x.Id == file.FolderId).SingleAsync();
                    var newFolder = await _context.Folders.Where(x => x.Id == folderId).SingleAsync();
                    if(oldFolder != null && newFolder != null)
                    {
                        oldFolder.Files.Remove(file);
                        newFolder.Files.Add(file);
                    }
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task<List<UserFile>> GetFilesForBlobUrls(List<string> blobUrls)
        {
            List<UserFile> files = new List<UserFile>();
            foreach (var blobUrl in blobUrls)
            {
                try
                {
                    var file = await _context.UserFiles.Include(x => x.Profile).Include(x => x.Topics).Where(x => blobUrl.StartsWith(x.BlobUri)).FirstAsync();
                    files.Add(file);
                }
                catch{ }
            }
            return files;
        }


        public async Task<List<UserFile>> GetFilesForUserAsync(string userId)
        {
            return await _context.UserFiles.Include(e => e.ProfileId).Where(e => e.ProfileId == userId).ToListAsync();
        }
        public async Task<UserFile> GetFilesForTitleAsync(string title)
        {
            try
            {
                return await _context.UserFiles.Where(e => e.Title.Contains(title)).FirstAsync();
            }
            catch
            {
                return new UserFile();
            }
        }

        public async Task<UserFile> GetFileAsync(int fileId)
        {
            try
            {
                return await _context.UserFiles.FirstAsync(e => e.Id == fileId);
            }
            catch
            {
                return null;
            }
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var file = await _context.UserFiles.FindAsync(fileId);
            if (file != null)
            {
                var blob = new BlobClient(new Uri(file.BlobUri));
                await blob.DeleteAsync();

                var folder = await _context.Folders
                    .Where(e => e.Id == file.FolderId)
                    .SingleAsync();
                if (folder != null)
                {
                    folder.Files.Remove(file);
                }
                var user = await _context.Profiles
                    .Where(e => e.Id == file.ProfileId)
                    .SingleAsync();
                if (user != null)
                {
                    user.UserItems.Remove(file);
                }

                _context.UserFiles.Remove(file);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<UserFile>> GetFilesForFolderAsync(int folderId)
        {
            return await _context.UserFiles
                .Include(e => e.Topics)
                .Where(f => f.FolderId == folderId)
                .ToListAsync();
        }

        public async Task<List<UserFile>> GetFilesForTopicAsync(int topicId)
        {
            var topic = await _context.Topics.FindAsync(topicId);
            return await _context.UserFiles
                .Where(f => f.Topics.Contains(topic))
                .ToListAsync();
        }

        private string GenerateBlobName(string fileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        }

    }
}