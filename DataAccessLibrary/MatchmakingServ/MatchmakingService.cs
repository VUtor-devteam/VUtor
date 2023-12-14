using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.MatchmakingServ
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
                return new List<ProfileEntity>();                                  //handles a case when no users are found
            }

            var possibleTutors = await _context.Profiles
                        .Include(profile => profile.TopicsToTeach)
                        .Where(profile => profile.Id != userId)                    //excludes the user from potential tutors
                        .ToListAsync();

            var matchedTutors = possibleTutors
                .Where(tutor => tutor.TopicsToTeach
                    .Any(topicTeach => user.TopicsToLearn
                        .Any(topicLearn => topicTeach.Title == topicLearn.Title)))
                .OrderByDescending(tutor => tutor.TopicsToTeach                     //sorts based on best match???
                    .Count(topicTeach => user.TopicsToLearn
                        .Any(topicLearn => topicTeach.Title == topicLearn.Title)))
                .ToList();

            return matchedTutors;
        }

        public async Task<List<ProfileEntity>> FindLearners(string userId)
        {
            var user = await _context.Users
                .Include(user => user.TopicsToTeach)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                return new List<ProfileEntity>();                                  //handles a case when no users are found
            }

            var possibleLearners = await _context.Profiles
                        .Include(profile => profile.TopicsToLearn)
                        .Where(profile => profile.Id != userId)                    //excludes the user from potential learners??
                        .ToListAsync();

            var matchedLearners = possibleLearners
                .Where(learner => learner.TopicsToLearn
                    .Any(topicLearn => user.TopicsToTeach
                        .Any(topicTeach => topicLearn.Title == topicTeach.Title)))
                .OrderByDescending(learner => learner.TopicsToLearn                     //sorts based on best match I think
                    .Count(topicLearn => user.TopicsToTeach
                        .Any(topicTeach => topicLearn.Title == topicTeach.Title)))
                .ToList();

            return matchedLearners;
        }

        public async Task<List<UserFile>> GetFilesForTopics(string userId) 
        {
            var user = await _context.Users
                .Include(user => user.TopicsToLearn)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                return new List<UserFile>();                      //handles a case when no files are found
            }

            var topics = user.TopicsToLearn.Select(topic => topic.Title).ToList();

            var matchedFiles = _context.UserFiles
                .Include(file => file.Topics)
                .Where(file => file.Topics.Any(fileTopic => topics.Contains(fileTopic.Title)))        //title?
                .OrderByDescending (file => file.CreationDate)
                .ToList(); 
            
            return matchedFiles;
        }
        public List<StudyGroup> GetRecentStudyGroups(int count)
        {
            var now = DateTime.Now;

            var recentGroups = _context.StudyGroups
                .Where(group => group.GroupDate > now)
                .OrderBy(group => group.GroupDate)
                .Take(count)
                .ToList();

            return recentGroups;
        }
    }
}