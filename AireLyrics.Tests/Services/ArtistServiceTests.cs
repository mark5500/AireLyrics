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

        // act
        var response = await client.SearchArtistByName("Ed Sheeran");

        // assert
        Assert.True(response.Artists.Any());
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
    public async Task SearchArtistByName_NotRealArtist_ReturnEmptyList()
    {
        // arrange
        var client = SetUpArtistService();

        // act
        var response = await client.SearchArtistByName("abcdefghijklmnopqrstuvwxyz");

        // assert
        Assert.False(response.Artists.Any());
    }
}
