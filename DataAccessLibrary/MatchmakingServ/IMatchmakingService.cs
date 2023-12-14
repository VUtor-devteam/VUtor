using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.MatchmakingServ
{
    public interface IMatchmakingService
    {
        Task<List<ProfileEntity>> FindTutors(string userId);
        Task<List<ProfileEntity>> FindLearners(string userId);
        Task<List<UserFile>> GetFilesForTopics(string userId);
        List<StudyGroup> GetRecentStudyGroups(int count);
    }
}