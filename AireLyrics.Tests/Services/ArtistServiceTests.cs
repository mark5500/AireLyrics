using AireLyrics.Services;
using System.Net.Http;

namespace AireLyrics.Tests.Services;

public class ArtistServiceTests
{
	private ArtistService SetUpArtistService()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://musicbrainz.org/ws/2/"),
            
        };

        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "AireLyrics/1.0.0 (https://github.com/mark5500)");

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(_ => _.CreateClient("ArtistApi"))
            .Returns(httpClient).Verifiable();

        return new ArtistService(httpClientFactory.Object);
    }

    [Fact]
    public async Task SearchArtistByName_ValidRequest_ReturnsResults()
    {
        // arrange
        var client = SetUpArtistService();
        var artistName = "Ed Sheeran";

        // act
        var response = await client.SearchArtistByName(artistName);

        // assert
        Assert.NotEmpty(response.Artists);
    }

    [Fact]
    public async Task SearchArtistByName_InvalidArtistName_ThrowsException()
    {
        // arrange
        var client = SetUpArtistService();

        // assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.SearchArtistByName(""));
    }

    [Fact]
    public async Task SearchArtistByName_IncorrectArtistName_ReturnEmptyList()
    {
        // arrange
        var client = SetUpArtistService();
        var artistName = "abcdefghijklmnopqrstuvwxyz";

        // act
        var response = await client.SearchArtistByName(artistName);

        // assert
        Assert.Empty(response.Artists);
    }

    [Fact]
    public async Task GetWorksByArtistId_ValidArtist_ReturnResults()
    {
        // arrange
        var client = SetUpArtistService();
        var artistId = Guid.Parse("b8a7c51f-362c-4dcb-a259-bc6e0095f0a6");

        // act
        var response = await client.GetWorksByArtistId(artistId);

        // assert
        Assert.NotEmpty(response.Works);
    }

    [Fact]
    public async Task GetWorksByArtistId_IncorrectArtistId_ReturnEmptyList()
    {
        // arrange
        var client = SetUpArtistService();
        var artistId = Guid.Parse("b8a7c51f-362c-4dcb-a265-bc6e0095f0a6");

        // act
        var response = await client.GetWorksByArtistId(artistId);

        // assert
        Assert.Empty(response.Works);
    }

    [Fact]
    public async Task GetWorksByArtistId_InvalidGuid_ThrowException()
    {
        // arrange
        var client = SetUpArtistService();
        var artistId = Guid.Empty;

        // assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.GetWorksByArtistId(artistId));
    }
}
