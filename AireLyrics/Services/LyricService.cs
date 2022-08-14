using System.Text.Json;
using Ardalis.GuardClauses;
using AireLyrics.Models;

namespace AireLyrics.Services;

public class LyricService : ILyricService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LyricService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<SearchLyricsResponse> SearchLyrics(string artistName, string songTitle)
    {
        Guard.Against.NullOrWhiteSpace(artistName, nameof(artistName));
        Guard.Against.NullOrWhiteSpace(songTitle, nameof(songTitle));
        
        HttpClient client = _httpClientFactory.CreateClient("LyricsApi");

        var url = $"{artistName}/{songTitle}";
        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            // deseralize content and return
            SearchLyricsResponse? result =  JsonSerializer.Deserialize<SearchLyricsResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is not null)
            {
                return result;
            }
        }

        return new SearchLyricsResponse();
    }
}
