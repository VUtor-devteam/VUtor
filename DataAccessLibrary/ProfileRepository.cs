﻿using DataAccessLibrary;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DataAccessLibrary
{
    public class ProfileRepository : GenericRepository<ProfileEntity>, IProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private Semaphore _pool;
        public ProfileRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;

            if (!Semaphore.TryOpenExisting("DbContextSemaphore", out _pool))
                _pool = new Semaphore(1, 1, "DbContextSemaphore");
        }

        public async Task<List<ProfileEntity>> GetProfilesByNameAsync(string name, string surname)
        {
            List<ProfileEntity> profiles = await LoadData();

            if (!profiles.IsNullOrEmpty())
            {
                while (!_pool.WaitOne(TimeSpan.FromTicks(1)))
                    await Task.Delay(TimeSpan.FromSeconds(1));
                profiles = profiles.Where(profile =>
                    (string.IsNullOrWhiteSpace(name) || (profile.Name != null && profile.Name.Contains(name, StringComparison.OrdinalIgnoreCase))) &&
                    (string.IsNullOrWhiteSpace(surname) || (profile.Surname != null && profile.Surname.Contains(surname, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }
            _pool.Release();

            return profiles;
        }

        public async Task<List<ProfileEntity>> GetProfilesByFilterAsync(string name, string surname, int courseName, int courseYear, int topicsLearn, int topicsTeach)
        {
            List<ProfileEntity> profiles = await LoadData();

            if (!profiles.IsNullOrEmpty())
            {
                while (!_pool.WaitOne(TimeSpan.FromTicks(1)))
                    await Task.Delay(TimeSpan.FromSeconds(1));

                profiles = (List<ProfileEntity>)profiles.FilterProfiles(name, surname, courseName, courseYear, topicsLearn, topicsTeach);
            }
            _pool.Release();

            return profiles;
        }

        public async Task<ProfileEntity> GetProfileByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            while (!_pool.WaitOne(TimeSpan.FromTicks(1)))
                await Task.Delay(TimeSpan.FromSeconds(1));

            try
            {
                // Updated query without StringComparison
                var profile = await _context.Profiles
                                            .Include(p => p.TopicsToLearn)  // Include TopicsToLearn
                                            .Include(p => p.TopicsToTeach)  // Include TopicsToTeach
                                            .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
                return profile;
            }
            finally
            {
                _pool.Release();
            }
        }
    }
}