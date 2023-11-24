using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public interface IMatchmakingService
    {
        Task<List<ProfileEntity>> FindTutors(string userId);
    }
}