using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly ApplicationDbContext _context;

        public MatchmakingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProfileEntity>> FindTutors(string userId)
        {
            var user = await _context.Users
                .Include(user => user.TopicsToLearn)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                return new List<ProfileEntity>(); //handles a case when no users are found
            }

            var tutors = await _context.Profiles
                        .Include(profile => profile.TopicsToTeach)
                        .Where(profile => profile.Id != userId) // Exclude the user from potential tutors
                        .ToListAsync();

            var matchedTutors = tutors
                .Where(tutor => tutor.TopicsToTeach
                    .Any(topicTeach => user.TopicsToLearn
                        .Any(topicLearn => topicTeach.Title == topicLearn.Title)))
                .ToList();

            return matchedTutors;
        }

    }
}