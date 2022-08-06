using AireLyrics.Models;

namespace AireLyrics.Services;

public class FakeArtistService : IArtistService
{
    public async Task<IEnumerable<Artist>> SearchArtistByName(string name)
    {
        // Simulate API call 
        await Task.Delay(1000);

        return new List<Artist>
        {
            new Artist("Anne-Marie"),
            new Artist("Ariana Grande"),
            new Artist("Ed Sheeran"),
            new Artist("Harry Styles"),
            new Artist("Jason Derulo"),
            new Artist("Little Mix"),
            new Artist("Miley Cyrus"),
        };
    }
}
