using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.FolderRepo
{
    public class FolderRepository : IFolderRepository
    {
        private readonly ApplicationDbContext _context;
        public FolderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> AddFilesToFolder(int folderId, List<UserFile> files)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddFileToFolder(int folderId, UserFile file)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddSubFolder(int folderId, int subFolderId)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddSubFolders(int folderId, List<int> subFolders)
        {
            throw new NotImplementedException();
        }

        public async Task<Folder> CreateFolder(string name, int? parentFolderId)
        {
            Folder parentFolder = null;
            if (parentFolderId != null)
            {
                try
                {
                    parentFolder = await _context.Folders.Where(e => e.ParentFolderId == parentFolderId).FirstAsync();
                }
                catch { }
            }

            if (parentFolder == null)
            {
                var folder = new Folder
                {
                    Name = name,
                    Path = name
                };
                await _context.Folders.AddAsync(folder);
                await _context.SaveChangesAsync();
                return folder;
            }
            else
            {
                var folder = new Folder
                {
                    Name = name,
                    Path = Path.Combine(parentFolder.Path, "\\", name),
                    ParentFolderId = parentFolder.Id,
                    ParentFolder = parentFolder
                };
                Console.WriteLine(Path.Combine(parentFolder.Path, "\\", name));
                await _context.Folders.AddAsync(folder);
                await _context.SaveChangesAsync();
                return folder;
            }
        }

        public async Task<Folder> CreateFolder(string name)
        {
            var folder = new Folder
            {
                Name = name,
                Path = name
            };
            await _context.Folders.AddAsync(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        public async Task<Folder> CreateSubFolder(string name, int parentFolderId)
        {
            Folder parentFolder = null;
            if (parentFolderId != null)
            {
                try
                {
                    parentFolder = await _context.Folders.Where(e => e.Id == parentFolderId).FirstAsync();
                }
                catch { }
                Console.WriteLine(parentFolder.Name);
                if (parentFolder != null)
                {
                    var folder = new Folder
                    {
                        Name = name,
                        Path = Path.Combine(parentFolder.Path, name),
                        ParentFolderId = parentFolder.Id,
                        ParentFolder = parentFolder
                    };
                    Console.WriteLine(Path.Combine(parentFolder.Path, name));
                    await _context.Folders.AddAsync(folder);
                    await _context.SaveChangesAsync();
                    return folder;
                }
            }
            return null;
        }

        public async Task DeleteFolder(int id)
        {
            var folder = await _context.Folders.Include(e => e.SubFolders).Where(e => e.Id == id).SingleAsync();
            if (folder != null)
            {
                if (folder.ParentFolderId != null)
                {
                    try
                    {
                        var parentFolder = await _context.Folders.Include(e => e.SubFolders).Where(e => e.ParentFolderId == folder.ParentFolderId).FirstAsync();
                    }
                    catch { }
                }
                if (folder.SubFolders != null)
                {
                    foreach (var subFolder in folder.SubFolders)
                    {
                        await DeleteFolder(subFolder.Id);
                    }
                }
                _context.Folders.Remove(folder);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Folder> GetFolder(int id)
        {
            return await _context.Folders.Include(x => x.SubFolders).Include(x => x.Files).Where(x => x.Id == id).SingleAsync();
        }

        public async Task<List<Folder>> GetFolders()
        {
            return await _context.Folders
            .Include(x => x.SubFolders)
            .Include(e => e.Files)
            .ToListAsync();
        }

        public async Task<List<Folder>> GetParentFolders()
        {
            return await _context.Folders
            .Include(x => x.SubFolders)
            .Include(x => x.Files)
            .Where(x => x.ParentFolderId == null)
            .ToListAsync();
        }
        public async Task<List<Folder>> GetSubFolders(int id)
        {
            List<Folder> subFolders = new List<Folder>();
            var folder = await GetFolder(id);
            foreach (var subFolder in folder.SubFolders)
            {
                var sub = await GetFolder(subFolder.Id);
                subFolders.Add(sub);
            }
            return subFolders;
        }

        public Task MoveFolder(int folderId, int newParentId)
        {
            throw new NotImplementedException();
        }
    }
}
