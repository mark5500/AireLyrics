using AireLyrics.Models;

namespace AireLyrics.Services;

public interface IArtistService 
{
    Task<IEnumerable<Artist>> SearchArtistByName(string name, int maxResults = 5);
}
