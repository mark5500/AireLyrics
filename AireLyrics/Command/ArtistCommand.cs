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
        public int Id { get; set; } = 0;
    }

    public ArtistCommand(IArtistService artistService)
    {
        _artistService = artistService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ArtistSettings settings)
    {
        AnsiConsole.Write(new FigletText("AireLyrics").Color(Color.Red));

        // index each result and print to screen
        IEnumerable<Artist> results = await GetArtist(settings.Name);
        var indexedArtists = results
            .Select((artist, index) => new { artist, index })
            .ToDictionary(x => x.index + 1, x => x.artist);

        if (!results.Any()) {
            AnsiConsole.MarkupLine("[red]No results, please try again.[/]\n");
            return 0;
        }

        PrintArtists(indexedArtists);

        int selectedArtistId = 0;
        if (results.Count() > 1 && settings.Id is 0)
        {
            selectedArtistId = AnsiConsole.Ask<int>("Please select [yellow]artist Id[/] from the list:");
        }
        else if (results.Count() > 1 && settings.Id is not 0)
        {
            selectedArtistId = settings.Id;
        }

        Artist? selectedArtist;
        if (indexedArtists.TryGetValue(selectedArtistId, out selectedArtist))
        {
            AnsiConsole.MarkupLine($"You have selected [aqua]{selectedArtist.Name}[/]\n");
        }

        // TODO: get list of artist works
        // TODO: calculate average word length of lyrics

        return 0;
    }

    /// <summary>
    /// Prompts user for artist name, if not already specified and calls API to search artist
    /// </summary>
    /// <param name="name"></param>
    /// <returns>List of artists</returns>
    private async Task<IEnumerable<Artist>> GetArtist(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // Ensure that the user has entered a valid string
            name = AnsiConsole.Ask<string>("Please enter an [yellow]artist name[/]: ");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Searching for artist:[/] [green]" + name + "[/]");
        }

        var results = await _artistService.SearchArtistByName(name);

        return results;
    }

    /// <summary>
    /// Prints a list of artists with selection Ids.
    /// </summary>
    /// <param name="artists"></param>
    private void PrintArtists(Dictionary<int, Artist> artists)
    {
        // display list of results for the user
        int index = 0;
        foreach (var artist in artists)
        {
            index++;
            AnsiConsole.Markup($"   [gray]{artist.Key}.[/] [cyan]{artist.Value.Name}[/]");

            if (!string.IsNullOrWhiteSpace(artist.Value.Country))
                AnsiConsole.Markup($" [gray]({artist.Value.Country})[/]");

            AnsiConsole.WriteLine();
        }

        AnsiConsole.WriteLine();
    }
}

