using AireLyrics.Models;
using AireLyrics.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AireLyrics.Command;

public class ArtistCommand : AsyncCommand<ArtistCommand.ArtistSettings>
{
    private readonly IArtistService _artistService;
    private readonly ILyricService _lyricService;

    public sealed class ArtistSettings : CommandSettings
    {
        [CommandOption("-a|--artist <NAME>")]
        [Description("Artist to search lyrics for.")]
        public string Name { get; set; } = string.Empty;

        [CommandOption("-s|--sampleSize <SAMPLESIZE>")]
        [Description("The amount of works that should be sampled for calculating word count. (Default: 10)")]
        public int SampleSize { get; set; } = 10;

        [CommandOption("-i|--id <ID>")]
        [Description("Artist search result Id.")]
        public int? Id { get; set; }
    }

    public ArtistCommand(IArtistService artistService, ILyricService lyricService)
    {
        _artistService = artistService;
        _lyricService = lyricService;
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

        var works = await GetWorksByArtistId(selectedArtist.Id, settings.SampleSize);
        AnsiConsole.MarkupLine($"Retreived list of {works.Count()} works successfully.\n");

        // TODO: calculate average word length of lyrics

        int wordCount = 0;
        await AnsiConsole.Progress()
             .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),    // Task description
                new ProgressBarColumn(),        // Progress bar
                new PercentageColumn(),         // Percentage
                new SpinnerColumn(),            // Spinner
            })
            .StartAsync(async ctx =>
            {
                ProgressTask task = ctx.AddTask($"[white]Fetching lyrics[/]");
                double incrementSize = 100.00 / settings.SampleSize;

                foreach (Work work in works)
                {
                    var response = await _lyricService.SearchLyrics(selectedArtist.Name, work.Title);

                    wordCount += CalculateWordCount(response.Lyrics);
                    task.Increment(incrementSize);
                }
            });

        var averageWords = wordCount / settings.SampleSize;
        AnsiConsole.MarkupLine($"[yellow]Retrieved {settings.SampleSize} works by {selectedArtist.Name}. The average word count is {averageWords}.[/]\n");
        return 0;
    }

    /// <summary>
    /// Calculates the word count of a string
    /// </summary>
    /// <param name="lyrics"></param>
    /// <returns></returns>
    private int CalculateWordCount(string lyrics)
    {
        if (string.IsNullOrEmpty(lyrics))
        {
            return 0;
        }

        return Regex.Matches(lyrics, @"[^\s]+").Count;
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
    /// <param name="sampleSize"></param>
    /// <returns>List of artists</returns>
    private async Task<List<Work>> GetWorksByArtistId(Guid artistId, int sampleSize = 100)
    {        
        var results = await _artistService.GetWorksByArtistId(artistId, sampleSize);
     
        List<Work> works = results.Works;
     
        AnsiConsole.MarkupLine($"[yellow]Found {results.WorkCount} works.[/]");

        while (works.Count < sampleSize) 
        {
            AnsiConsole.MarkupLine($"[gray]Fetching batch {works.Count()}-{works.Count() + sampleSize}[/]");
            var additionalResults = await _artistService.GetWorksByArtistId(artistId, sampleSize, works.Count());
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

