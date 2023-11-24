using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DataAccessLibrary.WebSearch;
using Microsoft.Extensions.Configuration;
using ServiceStack;
using System.Text;

namespace DataAccessLibrary.Search
{
    public class Search : ISearch
    {
        private readonly IConfiguration _config;
        public SearchClient _searchClient;

        public Search(IConfiguration configuration, SearchClient search)
        {
            _searchClient = search;
            _config = configuration;
        }
        public Search(IConfiguration configuration)
        {
            _config = configuration;

            string serviceName = _config.GetValue<string>("AzureSearchServiceName");
            string indexName = _config.GetValue<string>("AzureSearchIndexName");
            string queryApiKey = _config.GetValue<string>("AzureSearchQeuryKey");

            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(queryApiKey);

            _searchClient = new SearchClient(serviceEndpoint, indexName, credential);
        }

        public async Task<SearchResults<SearchDocument>> SearchDocumentsAsync(string searchText)
        {
            if (!searchText.IsNullOrEmpty())
            {
                SearchOptions options = new SearchOptions
                {
                    IncludeTotalCount = true,
                    Size = 10, // number of results to retrieve
                    QueryType = SearchQueryType.Full, // enable semantic search
                };
                options.Select.Add("*"); // fields to retrieve
                return await _searchClient.SearchAsync<SearchDocument>(searchText, options);
            }
            return null;
        }

        public List<string> GetBlobUrls(SearchResults<SearchDocument> searchResults)
        {
            var blobUrls = new List<string>();

            foreach (var result in searchResults.GetResults())
            {
                if (result.Document.TryGetValue("metadata_storage_path", out var path))
                {
                    string base64EncodedPath = path.ToString();
                    byte[] data = Convert.FromBase64String(base64EncodedPath);
                    string decodedPath = Encoding.UTF8.GetString(data);
                    blobUrls.Add(decodedPath);
                }
            }
            return blobUrls;
        }
    }
}
