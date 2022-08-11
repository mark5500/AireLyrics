using AireLyrics.Models;

namespace AireLyrics.Services;

public interface IArtistService 
{
    Task<SearchArtistResponse> SearchArtistByName(string name, int maxResults = 5);
    Task<GetWorksResponse> GetWorksByArtistId(Guid id, int limit = 100, int offset = 0);
}
