using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.MatchmakingServ
{
    public interface IMatchmakingService
    {
        Task<List<ProfileEntity>> FindTutorsAsync(string userId);
        Task<List<ProfileEntity>> FindLearnersAsync(string userId);
        Task<List<UserFile>> GetFilesForTopicsAsync(string userId);
        List<StudyGroup> GetRecentStudyGroups(int count);
    }
}