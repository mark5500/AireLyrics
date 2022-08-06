using AireLyrics.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AireLyrics.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IHttpClientFactory _httpClientFactory = null!;
        private readonly ILogger<ArtistService> _logger;

        public ArtistService(IHttpClientFactory httpClientFactory, ILogger<ArtistService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of artists matching the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Artist>> SearchArtistByName(string name, int maxResults = 5)
        {
            HttpClient client = _httpClientFactory.CreateClient("ArtistApi");

            try
            {
                var url = $"artist?query={name}&limit={maxResults}";
                var response = await client.GetAsync(url);

                // deserialize response on success and return list of artists
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ArtistSearchResult? result = JsonSerializer.Deserialize<ArtistSearchResult>(content, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result is not null)
                        return result.Artists;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured when trying to connect to ArtistApi", ex);
            }

            return new List<Artist>();
        }
    }
}
