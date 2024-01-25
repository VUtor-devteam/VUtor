using Azure.Storage.Blobs.Models;
using DataAccessLibrary.Data;
using DataAccessLibrary.GenericRepo;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DataAccessLibrary.ProfileRepo
{
    public class ProfileRepository : GenericRepository<ProfileEntity>, IProfileRepository
    {
        private readonly ApplicationDbContext _context;
        public ProfileRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ProfileEntity> GetProfilesByIdAsync(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var profile = await _context.Profiles
                    .AsSplitQuery()
                    .Include(x => x.TopicsToLearn)
                    .Include(x => x.TopicsToTeach)
                    .Include(x => x.UserItems)
                    .Where(x => x.Id == id).SingleAsync();
                return profile;
            }
            return null;
        }
        public async Task<List<ProfileEntity>> GetProfilesByNameAsync(string name, string surname)
        {
            List<ProfileEntity> profiles = new List<ProfileEntity>();

            if (!profiles.IsNullOrEmpty())
            {
                profiles = await _context.Profiles
                    .AsSplitQuery()
                    .Include(x => x.TopicsToLearn)
                    .Include(x => x.TopicsToTeach)
                    .Include(x => x.UserItems)
                    .Where(profile => (string.IsNullOrWhiteSpace(name) || profile.Name != null && profile.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(surname) || profile.Surname != null && profile.Surname.Contains(surname, StringComparison.OrdinalIgnoreCase)))
                    .ToListAsync();
            }
            return profiles;
        }

        public async Task<List<ProfileEntity>> GetProfilesByFilterAsync(string name, string surname, int courseName, int courseYear, int topicsLearn, int topicsTeach)
        {
            List<ProfileEntity> profiles = await LoadData();

            if (!profiles.IsNullOrEmpty())
            {
                profiles = (List<ProfileEntity>)profiles
                                            .FilterProfiles(name, surname, courseName, courseYear, topicsLearn, topicsTeach);
            }
            return profiles;
        }

        public async Task<ProfileEntity> GetProfileByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            try
            {
                // Updated query without StringComparison
                var profile = await _context.Profiles
                                            .AsSplitQuery()
                                            .Include(p => p.TopicsToLearn)  // Include TopicsToLearn
                                            .Include(p => p.TopicsToTeach)  // Include TopicsToTeach
                                            .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
                return profile;
            }
            finally { }
        }

        public async Task<ProfileEntity> GetProfileByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            try
            {
                // Updated query without StringComparison
                var profile = await _context.Profiles
                                            .AsSplitQuery()
                                            .Include(p => p.TopicsToLearn)  // Include TopicsToLearn
                                            .Include(p => p.TopicsToTeach)  // Include TopicsToTeach
                                            .FirstOrDefaultAsync(p => p.Id.ToLower() == id.ToLower());
                return profile;
            }
            finally { }
        }
    }
}