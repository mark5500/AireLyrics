using AireLyrics.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AireLyrics.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IHttpClientFactory _httpClientFactory = null!;

        public ArtistService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Returns a list of artists matching the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public async Task<SearchArtistResponse> SearchArtistByName(string name, int maxResults = 10)
        {
            HttpClient client = _httpClientFactory.CreateClient("ArtistApi");

            var url = $"artist?query={name}&limit={maxResults}";
            var response = await client.GetAsync(url);

            // deserialize response on success and return list of artists
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                SearchArtistResponse? result = JsonSerializer.Deserialize<SearchArtistResponse>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result is not null)
                    return result;
            }

            throw new ArgumentException("Invalid artist name");
        }

        /// <summary>
        /// Returns a list of works for given artist Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GetWorksResponse> GetWorksByArtistId(Guid id, int limit = 100, int offset = 0)
        {
            HttpClient client = _httpClientFactory.CreateClient("ArtistApi");

            var url = $"work?artist={id}&limit={limit}&offset={offset}";
            var response = await client.GetAsync(url);

            // deserialize response on success and return list of artists
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                GetWorksResponse? result = JsonSerializer.Deserialize<GetWorksResponse>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result is not null)
                {
                    return result;
                }
            }

            return new GetWorksResponse();
        }
    }
}
