using DataAccessLibrary.Models;

namespace DataAccessLibrary
{
    public interface IProfileRepository
    {
        Task<ProfileEntity> GetProfilesByIdAsync(string id);
        Task<List<ProfileEntity>> GetProfilesByNameAsync(string name, string surname);
        Task<List<ProfileEntity>> GetProfilesByFilterAsync(string name, string surname, int courseName, int courseYear, int topicsLearn, int topicsTeach);
        Task Delete(ProfileEntity entity);
        Task<IQueryable<ProfileEntity>> GetQueryable();
        Task Insert(ProfileEntity entity);
        Task<List<ProfileEntity>> LoadData();
        Task SaveChanges();
        Task Update(ProfileEntity entity);
        Task<ProfileEntity> GetProfileByEmailAsync(string email);
        Task<ProfileEntity> GetProfileByIdAsync(string id);
    }
}