using AireLyrics.Models;

namespace AireLyrics.Services;

public interface IArtistService 
{
    Task<List<Artist>> SearchArtistByName(string name, int maxResults = 5);
}
