using Newtonsoft.Json.Linq;
using ServiceStack;
using DataAccessLibrary.Models;
using DataAccessLibrary.MetaAPI.Exceptions;
using Newtonsoft.Json;
using LoggingConfiguration;
using Serilog;

namespace DataAccessLibrary.MetaAPI;

internal class MetaAPI : IMetaAPI
{
    private readonly ILogger _logger;

    static string appID = "352817443957233";
    static string appSecret = "51460ea67a90fda17b3d01feaadb78be";

    FacebookPage VUSA = new FacebookPage("VU SA MIF", "VUSAMIF");
    FacebookPage MIDI = new FacebookPage("MIDI", "midi.lt");

    string accessToken;

    public async void obtainAccessToken()
    {
        HttpClient client = new HttpClient();
        string tokenUrl = $"https://graph.facebook.com/oauth/access_token?client_id={appID}&client_secret={appSecret}&grant_type=pages_read_engagement";

        try
        {
            var response = await client.GetAsync(tokenUrl);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                var tokenData = JObject.Parse(tokenResponse);

                this.accessToken = tokenData["access_token"].ToString();
            }
            else
            {
                throw new InvalidClientResponseException("Error obtaining access token.");
            }
        }
        catch(Exception e)
        {
            _logger.Error(e, "An error occured: {ErrorMessage}", e.Message);
        }
    }   

    public async void renewAccessToken()
    {
        HttpClient client = new HttpClient();
        string debugTokenUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={accessToken}";

        try
        {
            var response = await client.GetAsync(debugTokenUrl);
            if (response.IsSuccessStatusCode)
            {
                var tokenInfo = await response.Content.ReadAsStringAsync();
                var tokenData = JObject.Parse(tokenInfo);

                bool isValid = tokenData["data"]["is_valid"].ToObject<bool>();
                if (!isValid)
                {
                    obtainAccessToken();
                }
            }
            else
            {
                throw new InvalidClientResponseException("Error checking access token validity.");
            }
        }
        catch(Exception e)
        {
            _logger.Error(e, "An error occured: {ErrorMessage}", e.Message);
        }
    }

    public async Task<List<Announcement>> FetchFacebookPagePosts(DateTime sinceDate, FacebookPage page)
    {
        long unixSinceDate = (long)(sinceDate - new DateTime(1970, 1, 1)).TotalSeconds;

        HttpClient client = new HttpClient();
        string postsUrl = $"https://graph.facebook.com/{page.PageID}/posts?access_token={accessToken}&since={unixSinceDate}";

        try
        {
            var response = await client.GetAsync(postsUrl);
            if (response.IsSuccessStatusCode)
            {
                var postData = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Announcement>>(postData);
            }
            else
            {
                throw new InvalidClientResponseException("Error loading posts");
            }
        }
        catch(Exception e)
        {
            _logger.Error(e, "An error occured: {ErrorMessage}", e.Message);
            return null;
        }
    }

    public async Task<List<Announcement>> fetchFacebookAnnouncements(DateTime sinceDate)
    {
        var posts = (await FetchFacebookPagePosts(sinceDate, MIDI)).Concat(await FetchFacebookPagePosts(sinceDate, VUSA)).ToList();

        try
        {
            if (posts != null)
            {
                return posts;
            }
            else
            {
                throw new ArgumentNullException(nameof(posts));
            }
        }
        catch(Exception e)
        {
            _logger.Error(e, "An error occured: {ErrorMessage}", e.Message);
            return null;
        }
    }
}

public struct FacebookPage
{
    public string Name { get; set; }
    public string PageID { get; set; }

    public FacebookPage(string name, string pageID)
    {
        this.Name = name;
        this.PageID = pageID;
    }
}

