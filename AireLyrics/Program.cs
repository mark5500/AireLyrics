// Set up host and register services
using AireLyrics.Command;
using AireLyrics.Infrastructure;
using AireLyrics.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var registrations = new ServiceCollection();
registrations.AddHttpClient("ArtistApi", client =>
{
    client.BaseAddress = new Uri("https://musicbrainz.org/ws/2/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "AireLyrics/1.0.0 (https://github.com/mark5500)");
});

registrations.AddHttpClient("LyricsApi", client =>
{
    client.BaseAddress = new Uri("https://api.lyrics.ovh/v1/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

registrations.AddSingleton<IArtistService, ArtistService>();
registrations.AddSingleton<ILyricService, LyricService>();
var registrar = new TypeRegistrar(registrations);

var app = new CommandApp<ArtistCommand>(registrar);

return app.Run(args);