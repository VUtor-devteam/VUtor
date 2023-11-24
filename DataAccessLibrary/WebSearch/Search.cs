using Azure;
using System.Text;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Azure.Search.Documents.Indexes.Models;
using ServiceStack;
using DataAccessLibrary.WebSearch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Search
{
    // This class represents the search functionality
    public class Search : ISearch
    {
        private readonly IConfiguration _config;
        private readonly SearchClient _searchClient;

        // Constructor with dependency injection
        public Search(IConfiguration configuration, SearchClient searchClient)
        {
            _config = configuration;
            _searchClient = searchClient;
        }

        // Constructor without dependency injection
        public Search(IConfiguration configuration)
        {
            _config = configuration;

            // Get Azure Search service configuration values from appsettings.json
            string serviceName = _config.GetValue<string>("AzureSearchServiceName");
            string indexName = _config.GetValue<string>("AzureSearchIndexName");
            string queryApiKey = _config.GetValue<string>("AzureSearchQeuryKey");

            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(queryApiKey);

            // Create a new instance of SearchClient
            _searchClient = new SearchClient(serviceEndpoint, indexName, credential);
        }

        // Search documents based on the provided searchText
        public async Task<SearchResults<SearchDocument>> SearchDocumentsAsync(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                SearchOptions options = new SearchOptions
                {
                    IncludeTotalCount = true,
                    Size = 10, // Number of results to retrieve
                    QueryType = SearchQueryType.Full, // Enable semantic search
                };
                options.Select.Add("*"); // Fields to retrieve

                // Perform the search and return the results
                return await _searchClient.SearchAsync<SearchDocument>(searchText, options);
            }

            return null;
        }

        // Get the blob URLs from the search results
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
