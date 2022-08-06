// Set up host and register services
using AireLyrics.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

await BuildCommandLine()
    .UseHost(_ => Host.CreateDefaultBuilder(), host =>
    {
        host.ConfigureServices(services =>
        {
            services.AddSingleton<IArtistService, FakeArtistService>();
            services.AddSingleton<App>();
        });
    })
    .UseDefaults()
    .Build()
    .InvokeAsync(args);

// Here we build our command line with a required artist option
static CommandLineBuilder BuildCommandLine()
{
    var root = new RootCommand("Search for an artist.")
    {
        new Option<string>("--artist")
        {
            Name = "Artist",
            Description = "The name of the artist to search for."
        }
    };

    root.Handler = CommandHandler.Create<string, IHost>(Run);
    return new CommandLineBuilder(root);
}

static async Task Run(string artist, IHost host)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new FigletText("AireLyrics").Color(Color.Red));
    AnsiConsole.MarkupLine("[gray]A tool to find the average lyric count of a music artist.[/]");
    Console.WriteLine("");

    if (string.IsNullOrWhiteSpace(artist))
    {
        // Ensure that the user has entered a valid string
        artist = AnsiConsole.Ask<string>("\n\n\nPlease enter an [yellow]artist name[/]: ");
    } 
    else
    {
        AnsiConsole.MarkupLine("[yellow]Searching for artist:[/] [green]" + artist + "[/]");
    }

    var app = host.Services.GetRequiredService<App>();
    await app.Run(artist);
}