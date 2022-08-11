using AireLyrics.Command;
using AireLyrics.Models;
using AireLyrics.Services;
using Moq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AireLyrics.Tests.Commands;

public class ArtistCommandTests
{
    private readonly List<Artist> _testArtists = new List<Artist>
    {
        new Artist(Guid.Parse("e3d5b5ec-101a-4529-9f2d-01dca64cf44e"), "Person", "Anne-Marie", "FR"),
        new Artist(Guid.Parse("afb680f2-b6eb-4cd7-a70b-a63b25c763d5"), "Person", "Bruno Mars", "US"),
        new Artist(Guid.Parse("cc197bad-dc9c-440d-a5b5-d52ba2e14234"), "Group", "Coldplay", "UK"),
        new Artist(Guid.Parse("b8a7c51f-362c-4dcb-a259-bc6e0095f0a6"), "Person", "Ed Sheeran", "UK"),
        new Artist(Guid.Parse("33ca19f4-18c8-4411-98df-ac23890ce9f5"), "Person", "Ellie Goulding", "UK"),
        new Artist(Guid.Parse("012151a8-0f9a-44c9-997f-ebd68b5389f9"), "Group", "Imagine Dragons", "US"),
        new Artist(Guid.Parse("875203e1-8e58-4b86-8dcb-7190faf411c5"), "Person", "J. Cole", "US")
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
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse {
                Artists = _testArtists
            });

        artistService.Setup(c => c.GetWorksByArtistId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new GetWorksResponse());

        var command = new ArtistCommand(artistService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, _settings);

        // assert
        Assert.Equal(0, result);

        var text = AnsiConsole.ExportText();
        Assert.Contains("Searching for artist: Beyonce", text);
        Assert.Contains("   1. Anne-Marie (FR)", text);
        Assert.Contains("   2. Bruno Mars (US)", text);
        Assert.Contains("   3. Coldplay (UK)", text);
        Assert.Contains("   4. Ed Sheeran (UK)", text);
        Assert.Contains("   5. Ellie Goulding (UK)", text);
    }

    [Fact]
    public async Task Execute_NoResults_Exits()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse());
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

    [Fact]
    public async Task Execute_IdSpecified_GetsArtist()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse {
                Artists = _testArtists
            });

        artistService.Setup(c => c.GetWorksByArtistId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new GetWorksResponse());

        var command = new ArtistCommand(artistService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        var settings = new ArtistCommand.ArtistSettings
        {
            Name = "Anne-Marie",
            Id = 1
        };
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, settings);

        // assert
        Assert.Equal(0, result);

        var text = AnsiConsole.ExportText();
        Assert.Contains("You have selected: Anne-Marie", text);
    }

    [Fact]
    public async Task Execute_NoIdSpecified_OneResult_GetsArtist()
    {
        // arrange
        var artistService = new Mock<IArtistService>();
        artistService.Setup(c => c.SearchArtistByName(It.IsAny<string>(), 5))
            .ReturnsAsync(new SearchArtistResponse {
                    Artists = new List<Artist> 
                    { 
                        new Artist(Guid.Parse("859d0860-d480-4efd-970c-c05d5f1776b8"), "Person", "Beyonce", "US") 
                    }
                });

        artistService.Setup(c => c.GetWorksByArtistId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new GetWorksResponse());

        var command = new ArtistCommand(artistService.Object);
        var context = new CommandContext(_remainingArgs, "--artist", null);
        var settings = new ArtistCommand.ArtistSettings
        {
            Name = "Beyonce",
            Id = 0
        };
        AnsiConsole.Record();

        // act
        var result = await command.ExecuteAsync(context, settings);

        // assert
        Assert.Equal(0, result);

        var text = AnsiConsole.ExportText();
        Assert.Contains("You have selected: Beyonce", text);
    }
}