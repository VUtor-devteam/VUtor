namespace DataAccessLibrary.Models;

public class Connection
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public ProfileEntity User { get; set; }
    public string FriendId { get; set; }
    public ProfileEntity Friend { get; set; }
}

public class ConnectionRequest
{
    public Guid Id { get; set; }
    public string SenderId { get; set; }
    public ProfileEntity Sender { get; set; }
    public string ReceiverId { get; set; }
    public ProfileEntity Receiver { get; set; }
}