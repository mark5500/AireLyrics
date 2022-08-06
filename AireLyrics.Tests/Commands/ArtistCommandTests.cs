using AireLyrics.Command;
using AireLyrics.Models;
using AireLyrics.Services;
using Moq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AireLyrics.Tests.Commands;

public class ArtistCommandTests
{
    private readonly IEnumerable<Artist> _testArtists = new List<Artist>
    {
        new Artist("Anne-Marie", "France"),
        new Artist("Bruno Mars", "United States"),
        new Artist("Coldplay", "British"),
        new Artist("Ed Sheeran", "British"),
        new Artist("Ellie Goulding", "British"),
        new Artist("Imagine Dragons", "United States"),
        new Artist("J. Cole", "United States")
    };

    private readonly IRemainingArguments _remainingArgs = new Mock<IRemainingArguments>().Object;

    private readonly ArtistCommand.ArtistSettings _settings = new ArtistCommand.ArtistSettings
    {
        Name = "Beyonce",
        Id = 1
    };

    [Fact]
    public async Task Execute_WithResults_ShowsResults()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5)).ReturnsAsync(_testArtists);
        var command = new ArtistCommand(artistService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, _settings);

        // assert
        Assert.Equal(0, result);

        var text = AnsiConsole.ExportText();
        Assert.Contains("Searching for artist: Beyonce", text);
        Assert.Contains("   1. Anne-Marie (France)", text);
        Assert.Contains("   2. Bruno Mars (United States)", text);
        Assert.Contains("   3. Coldplay (British)", text);
        Assert.Contains("   4. Ed Sheeran (British)", text);
        Assert.Contains("   5. Ellie Goulding (British)", text);
    }

    [Fact]
    public async Task Execute_NoResults_Exits()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5)).ReturnsAsync(new List<Artist>());
        var command = new ArtistCommand(artistService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, _settings);

        // assert
        Assert.Equal(0, result);

        var text = AnsiConsole.ExportText();
        Assert.Contains("No results, please try again.", text);
    }
}