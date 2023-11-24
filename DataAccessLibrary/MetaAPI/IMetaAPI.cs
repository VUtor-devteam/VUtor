using DataAccessLibrary.Models;

namespace DataAccessLibrary.MetaAPI;

public interface IMetaAPI
{
    Task<List<Announcement>> fetchFacebookAnnouncements(DateTime sinceDate);
    Task<List<Announcement>> FetchFacebookPagePosts(DateTime sinceDate, FacebookPage page);
    void obtainAccessToken();
    void renewAccessToken();
}