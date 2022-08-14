using AireLyrics.Command;
using AireLyrics.Models;
using AireLyrics.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AireLyrics.Tests.Commands;

public class ArtistCommandTests
{
    private readonly List<Artist> _testArtists = new List<Artist>
    {
        new Artist(Guid.Parse("e3d5b5ec-101a-4529-9f2d-01dca64cf44e"), "Person", "Test Artist 1", "FR"),
        new Artist(Guid.Parse("afb680f2-b6eb-4cd7-a70b-a63b25c763d5"), "Person", "Test Artist 2", "US"),
        new Artist(Guid.Parse("cc197bad-dc9c-440d-a5b5-d52ba2e14234"), "Group", "Test Artist 3", "UK"),
        new Artist(Guid.Parse("b8a7c51f-362c-4dcb-a259-bc6e0095f0a6"), "Person", "Test Artist 4", "UK"),
        new Artist(Guid.Parse("33ca19f4-18c8-4411-98df-ac23890ce9f5"), "Person", "Test Artist 5", "UK"),
    };

    private readonly List<Artist> _testArtist = new List<Artist>
    {
        new Artist(Guid.Parse("859d0860-d480-4efd-970c-c05d5f1776b8"), "Person", "Test Artist", "US")
    };

    private readonly IRemainingArguments _remainingArgs = new Mock<IRemainingArguments>().Object;

    private readonly ArtistCommand.ArtistSettings _settings = new ArtistCommand.ArtistSettings
    {
        Name = "Beyonce",
        Id = 1
    };
    private readonly GetWorksResponse _testWorks = new GetWorksResponse
    {
        WorkCount = 5,
        Works =
        {
            new Work("Test Song 1"),
            new Work("Test Song 2"),
            new Work("Test Song 3"),
            new Work("Test Song 4"),
            new Work("Test Song 5")
        }
    };

    [Fact]
    public async Task Execute_WithResults_ShowsResults()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse {
                Artists = _testArtists
            });

        artistService.Setup(c => c.GetWorksByArtistId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new GetWorksResponse());

        var lyricsService = new Mock<ILyricService>();
        lyricsService.Setup(c => c.SearchLyrics(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new SearchLyricsResponse());

        var command = new ArtistCommand(artistService.Object, lyricsService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, _settings);

        // assert
        var text = AnsiConsole.ExportText();
        Assert.Contains("Searching for artist: Beyonce", text);
        Assert.Contains("   1. Test Artist 1 (FR)", text);
        Assert.Contains("   2. Test Artist 2 (US)", text);
        Assert.Contains("   3. Test Artist 3 (UK)", text);
        Assert.Contains("   4. Test Artist 4 (UK)", text);
        Assert.Contains("   5. Test Artist 5 (UK)", text);
    }

    [Fact]
    public async Task Execute_NoResults_Exits()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse());

        var lyricsService = new Mock<ILyricService>();
        lyricsService.Setup(c => c.SearchLyrics(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new SearchLyricsResponse());

        var command = new ArtistCommand(artistService.Object, lyricsService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, _settings);

        // assert
        var text = AnsiConsole.ExportText();
        Assert.Contains("No results, please try again.", text);
    }

    [Fact]
    public async Task Execute_IdSpecified_GetsArtist()
    {
        // arrange
        var artistService = new Mock<IArtistService>();

        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse { Artists = _testArtists });
        artistService.Setup(c => c.GetWorksByArtistId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new GetWorksResponse());

        var lyricsService = new Mock<ILyricService>();
        lyricsService.Setup(c => c.SearchLyrics(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new SearchLyricsResponse());

        var command = new ArtistCommand(artistService.Object, lyricsService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        var settings = new ArtistCommand.ArtistSettings
        {
            Name = "Test Artist 1",
            Id = 1
        };
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, settings);

        // assert
        var text = AnsiConsole.ExportText();
        Assert.Contains("You have selected: Test Artist 1", text);
    }

    [Fact]
    public async Task Execute_NoIdSpecified_OneResult_GetsArtist()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse { Artists = _testArtist });

        artistService.Setup(c => c.GetWorksByArtistId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new GetWorksResponse());

        var lyricsService = new Mock<ILyricService>();
        lyricsService.Setup(c => c.SearchLyrics(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new SearchLyricsResponse());

        var command = new ArtistCommand(artistService.Object, lyricsService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        var settings = new ArtistCommand.ArtistSettings
        {
            Name = "Test Artist 1",
            Id = 0
        };
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, settings);

        // assert
        var text = AnsiConsole.ExportText();
        Assert.Contains("You have selected: Test Artist", text);
    }

    [Fact]
    public async Task Execute_WithResults_CalculatesWordCount()
    {
        // arrange
        var artistService = new Mock<IArtistService>();

        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse { Artists = _testArtists });
        artistService.Setup(c => c.GetWorksByArtistId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(_testWorks);

        var lyricsService = new Mock<ILyricService>();
        lyricsService.Setup(c => c.SearchLyrics(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new SearchLyricsResponse { Lyrics = "one two three four five" });

        var command = new ArtistCommand(artistService.Object, lyricsService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        var settings = new ArtistCommand.ArtistSettings
        {
            Name = "Test Artist 1",
            Id = 1,
            SampleSize = 20
        };
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, settings);

        // assert
        var text = AnsiConsole.ExportText();
        Assert.Contains("Retrieved lyrics for 20 works by Test Artist 1. The average word count is 5.", text);
    }
}