using DataAccessLibrary.Data;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.ConnectionRepo;

public class ConnectionsRepository : IConnectionsRepository
{
    private readonly ApplicationDbContext _context;

    public ConnectionsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task GenerateLists(ProfileEntity user)
    {
        if (user != null)
        {
            if (user.Connections == null)
            {
                user.Connections = new List<Connection>();
            }
            if (user.ConnectionRequests == null)
            {
                user.ConnectionRequests = new List<ConnectionRequest>();
            }
            await _context.SaveChangesAsync();
        }
    }   

    public async Task<List<Connection>> GetConnectionsByUserAsync(ProfileEntity user)
    {
        if (user != null)
        {
            var connections = user.Connections.ToList();

            return connections;
        }
        return null;
    }

    public async Task<List<ConnectionRequest>> GetSentConnectionRequestsByUserAsync(ProfileEntity user)
    {
        if (user != null)
        {
            var requestsSent = user.ConnectionRequests
            .Where(x => x.SenderId == user.Id)
            .ToList();

            return requestsSent;
        }
        return null;
    }

    public async Task<List<ConnectionRequest>> GetPendingConnectionRequestsByUserAsync(ProfileEntity user)
    {
        if (user != null)
        {
            var pendingRequests = user.ConnectionRequests
            .Where(x => x.ReceiverId == user.Id)
            .ToList();

            return pendingRequests;
        }
        return null;
    }

    public async Task<Connection> GetConnectionByUsers(ProfileEntity user, ProfileEntity friend)
    {
        if (await HaveConnection(user, friend))
        {
            var connection = user.Connections
                .FirstOrDefault(x => x.FriendId == friend.Id);

            return connection;
        }
        return null;
    }

    public async Task<ConnectionRequest> GetConnectionRequestByUsers(ProfileEntity user, ProfileEntity friend)
    {
        if (await HaveOutgoingRequest(user, friend))
        {
            var request = user.ConnectionRequests
                .Where(x => x.ReceiverId == friend.Id)
                .FirstOrDefault();

            return request;
        }
        return null;
    }

    public async Task SendConnectionRequest(ProfileEntity sender, ProfileEntity receiver)
    {
        if(sender != null && receiver != null)
        {
            ConnectionRequest request = new ConnectionRequest
            {
                Id = Guid.NewGuid(),
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Sender = sender,
                Receiver = receiver
            };
            if(sender.ConnectionRequests == null)
            {
                sender.ConnectionRequests = new List<ConnectionRequest>();
            }
            if(receiver.ConnectionRequests == null)
            {
                receiver.ConnectionRequests = new List<ConnectionRequest>();
            }
            sender.ConnectionRequests.Add(request);
            receiver.ConnectionRequests.Add(request);

            await _context.SaveChangesAsync();
        }
    }

    public async Task AcceptConnectionRequest(ConnectionRequest connectionRequest)
    {
        if (connectionRequest != null)
        {
            var sender = await _context.Users.FindAsync(connectionRequest.SenderId);
            var receiver = await _context.Users.FindAsync(connectionRequest.ReceiverId);

            var connectionSender = new Connection
            {
                Id = Guid.NewGuid(),
                User = connectionRequest.Sender,
                UserId = connectionRequest.SenderId,
                FriendId = connectionRequest.ReceiverId,
                Friend = connectionRequest.Receiver
            };

            var connectionReceiver = new Connection
            {
                Id = Guid.NewGuid(),
                User = connectionRequest.Receiver,
                UserId = connectionRequest.ReceiverId,
                FriendId = connectionRequest.SenderId,
                Friend = connectionRequest.Sender
            };

            sender.Connections.Add(connectionSender);
            receiver.Connections.Add(connectionReceiver);

            sender.ConnectionRequests.Remove(connectionRequest);
            receiver.ConnectionRequests.Remove(connectionRequest);

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveConnection(Connection connection)
    {
        if (connection != null)
        {
            var remover = await _context.Profiles.FindAsync(connection.UserId);
            var receiver = await _context.Profiles.FindAsync(connection.FriendId);

            if(remover != null || receiver != null)
            {
                await GenerateLists(remover);
                await GenerateLists(receiver);

                var receiverConnection = receiver.Connections
                .Where(x => x.FriendId == connection.UserId)
                .FirstOrDefault();

                remover.Connections.Remove(connection);
                receiver.Connections.Remove(receiverConnection);

                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task CancelRequest(ConnectionRequest request)
    {
        if (request != null)
        {
            var canceler = await _context.Profiles.FindAsync(request.SenderId);
            var receiver = await _context.Profiles.FindAsync(request.ReceiverId);

            if(canceler != null && receiver != null)
            {
                await GenerateLists(canceler);
                await GenerateLists(receiver);

                canceler.ConnectionRequests.Remove(request);
                receiver.ConnectionRequests.Remove(request);

                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<Boolean> HaveConnection(ProfileEntity user, ProfileEntity maybeFriend)
    {
        if (user == null || maybeFriend == null)
        {
            return false;
        }
        else if (user.Connections != null && maybeFriend.Connections != null)
        {

            var userConnection = user.Connections
                .FirstOrDefault(x => x.FriendId == maybeFriend.Id);
            var friendConnection = maybeFriend.Connections
                .FirstOrDefault(x => x.FriendId == user.Id);

            if (userConnection != null && friendConnection != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task<Boolean> HaveOutgoingRequest(ProfileEntity user, ProfileEntity maybeFriend)
    {
        if (user == null || maybeFriend == null)
        {
            return false;
        }
        else if(user.ConnectionRequests != null && maybeFriend.ConnectionRequests != null)
        {
            var userRequest = user.ConnectionRequests
                .FirstOrDefault(x => x.ReceiverId == maybeFriend.Id);

            if (userRequest != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
            
        }
    }
}
