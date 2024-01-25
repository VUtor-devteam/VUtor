using Azure.Search.Documents.Models;

namespace DataAccessLibrary.WebSearch
{
    public interface ISearch
    {
        Task<SearchResults<SearchDocument>> SearchDocumentsAsync(string searchText);
        List<string> GetBlobUrls(SearchResults<SearchDocument> searchResults);
    }
}
