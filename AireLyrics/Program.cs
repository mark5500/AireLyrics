// Set up host and register services
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    // Ensure that the user has entered a valid string
    while (string.IsNullOrWhiteSpace(artist))
    {
        Console.WriteLine("Please specify an artist to search for.");
        artist = Console.ReadLine();
    }

    var app = host.Services.GetRequiredService<App>();
    await app.Run(artist);
}