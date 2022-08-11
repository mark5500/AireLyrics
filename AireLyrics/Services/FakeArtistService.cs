using AireLyrics.Models;

namespace AireLyrics.Services;

public class FakeArtistService : IArtistService
{
    public async Task<List<Artist>> SearchArtistByName(string name, int maxResults)
    {
        // Simulate API call 
        await Task.Delay(1000);

        return new List<Artist>
        {
            new Artist("Anne-Marie", "France"),
            new Artist("Bruno Mars", "United States"),
            new Artist("Coldplay", "British"),
            new Artist("Ed Sheeran", "British"),
            new Artist("Ellie Goulding", "British"),
            new Artist("Imagine Dragons", "United States"),
            new Artist("J. Cole", "United States")
        }.Take(maxResults)
        .ToList();
    }
}
