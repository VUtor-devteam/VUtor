using DataAccessLibrary.Models;

namespace DataAccessLibrary.ConnectionRepo
{
    public interface IConnectionsRepository
    {
        Task AcceptConnectionRequest(ConnectionRequest connectionRequest);
        Task CancelRequest(ConnectionRequest request);
        Task<Connection> GetConnectionByUsers(ProfileEntity user, ProfileEntity friend);
        Task<ConnectionRequest> GetConnectionRequestByUsers(ProfileEntity user, ProfileEntity friend);
        Task<List<Connection>> GetConnectionsByUserAsync(ProfileEntity user);
        Task<List<ConnectionRequest>> GetPendingConnectionRequestsByUserAsync(ProfileEntity user);
        Task<List<ConnectionRequest>> GetSentConnectionRequestsByUserAsync(ProfileEntity user);
        Task<bool> HaveConnection(ProfileEntity user, ProfileEntity maybeFriend);
        Task<bool> HaveOutgoingRequest(ProfileEntity user, ProfileEntity maybeFriend);
        Task RemoveConnection(Connection connection);
        Task SendConnectionRequest(ProfileEntity sender, ProfileEntity receiver);
        Task GenerateLists(ProfileEntity user);
    }
}