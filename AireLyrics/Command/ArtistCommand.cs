using AireLyrics.Models;
using AireLyrics.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AireLyrics.Command;

public class ArtistCommand : AsyncCommand<ArtistCommand.ArtistSettings>
{
    private readonly IArtistService _artistService;

    public sealed class ArtistSettings : CommandSettings
    {
        [CommandOption("-a|--artist <NAME>")]
        [Description("Artist to search lyrics for.")]
        public string Name { get; set; } = string.Empty;

        [CommandOption("--id <ID>")]
        [Description("Artist search result Id.")]
        public int? Id { get; set; }
    }

    public ArtistCommand(IArtistService artistService)
    {
        _artistService = artistService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ArtistSettings settings)
    {
        AnsiConsole.Write(new FigletText("AireLyrics").Color(Color.Red));

        // index each result and print to screen
        List<Artist> artists = await SearchArtist(settings.Name);

        if (!artists.Any()) {
            AnsiConsole.MarkupLine("[red]No results, please try again.[/]\n");
            return 0;
        }

        PrintArtists(artists);

        var selectedIndex = 1;
        if (artists.Count > 1) 
        {
            // check command options or ask user for index
            if (settings.Id is not null) 
            {
                selectedIndex = settings.Id.Value;
            }
            else 
            {
                selectedIndex = AnsiConsole.Ask<int>("Please select [yellow]artist id[/] from the list:");
            }
        }

        // check if given value is a valid index of results
        Artist selectedArtist;
        if (selectedIndex >= 1 && selectedIndex < artists.Count + 1)
        {
            selectedArtist = artists[selectedIndex - 1];
            AnsiConsole.MarkupLine($"You have selected: [aqua]{selectedArtist.Name}[/]\n");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Selected artist not found, please try again.[/]\n");
            return 0;
        }

        var works = await GetWorksByArtistId(selectedArtist.Id);
        AnsiConsole.MarkupLine($"Retreived list of {works.Count()} works successfully.\n");

        // TODO: calculate average word length of lyrics

        return 0;
    }

    /// <summary>
    /// Prompts user for artist name, if not already specified and calls API to search artist
    /// </summary>
    /// <param name="name"></param>
    /// <returns>List of artists</returns>
    private async Task<List<Artist>> SearchArtist(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // Ensure that the user has entered a valid string
            name = AnsiConsole.Ask<string>("Please enter an [yellow]artist name[/]:");
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]Searching for artist:[/] [green]{name}[/]");
        }

        var results = await _artistService.SearchArtistByName(name);

        return results.Artists;
    }

    /// <summary>
    /// Retreives list of works matching artist Id
    /// </summary>
    /// <param name="artistId"></param>
    /// <param name="batchSize"></param>
    /// <returns>List of artists</returns>
    private async Task<List<Work>> GetWorksByArtistId(Guid artistId, int batchSize = 100)
    {        
        var results = await _artistService.GetWorksByArtistId(artistId);
     
        List<Work> works = results.Works;
        int totalWorks = results.WorkCount;
     
        AnsiConsole.MarkupLine($"[yellow]Found {results.WorkCount} works.[/]");

        while (works.Count < totalWorks) 
        {
            AnsiConsole.MarkupLine($"[gray]Fetching batch {works.Count()}-{works.Count() + batchSize}[/]");
            var additionalResults = await _artistService.GetWorksByArtistId(artistId, batchSize, works.Count());
            works.AddRange(additionalResults.Works);
        }
        
        return works;
    }

    /// <summary>
    /// Prints a list of artists with selection Ids.
    /// </summary>
    /// <param name="artists"></param>
    private void PrintArtists(List<Artist> artists)
    {
        // display list of results for the user
        int index = 0;
        foreach (var artist in artists)
        {
            index++;
            AnsiConsole.Markup($"   [gray]{index}.[/] [cyan]{artist.Name}[/]");

            if (!string.IsNullOrWhiteSpace(artist.Country))
                AnsiConsole.Markup($" [gray]({artist.Country})[/]");

            AnsiConsole.WriteLine();
        }

        AnsiConsole.WriteLine();
    }
}

