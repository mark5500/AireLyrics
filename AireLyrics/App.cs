// Set up host and register services
using AireLyrics.Services;

public class App
{
    private readonly IArtistService _artistService;

    public App(IArtistService artistService)
    {
        _artistService = artistService;
    }

    public async Task Run(string artist)
    {
        var results = await _artistService.SearchArtistByName(artist);

        
    }
}