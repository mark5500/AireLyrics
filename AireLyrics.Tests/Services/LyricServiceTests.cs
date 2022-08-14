using AireLyrics.Services;

namespace AireLyrics.Tests.Services;

public class LyricServiceTests
{
    private LyricService SetUpLyricService()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.lyrics.ovh/v1/"),

        };

        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(_ => _.CreateClient("LyricsApi"))
            .Returns(httpClient).Verifiable();

        return new LyricService(httpClientFactory.Object);
    }

    [Fact]
    public async Task SearchLyrics_ValidRequest_ReturnsLyrics()
    {
        // arrange
        var client = SetUpLyricService();
        var artistName = "Ed Sheeran";
        var songTitle = "Shape of You";

        // act
        var response = await client.SearchLyrics(artistName, songTitle);

        // assert
        Assert.False(string.IsNullOrWhiteSpace(response.Lyrics));
    }

    [Fact]
    public async Task SearchLyrics_IncorrectArtistName_ReturnsEmptyList()
    {
        // arrange
        var client = SetUpLyricService();
        var artistName = "abcdefghijklmnopqrstuvwxyz";
        var songTitle = "Shape of You";

        // act
        var response = await client.SearchLyrics(artistName, songTitle);

        // assert
        Assert.True(string.IsNullOrWhiteSpace(response.Lyrics));
    }
    
    [Fact]
    public async Task SearchLyrics_IncorrectSongTitle_ReturnsEmptyList()
    {
        // arrange
        var client = SetUpLyricService();
        var artistName = "Ed Sheeran";
        var songTitle = "abcdefghijklmnopqrstuvwxyz";

        // act
        var response = await client.SearchLyrics(artistName, songTitle);

        // assert
        Assert.True(string.IsNullOrWhiteSpace(response.Lyrics));
    }

    [Fact]
    public async Task SearchLyrics_InvalidArtistName_ThrowsException()
    {
        // arrange
        var client = SetUpLyricService();
        var artistName = "";
        var songTitle = "Shape of You";

        // assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.SearchLyrics(artistName, songTitle));
    }

    [Fact]
    public async Task SearchLyrics_InvalidSongTitle_ThrowsException()
    {
        // arrange
        var client = SetUpLyricService();
        var artistName = "Ed Sheeran";
        var songTitle = "";

        // assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.SearchLyrics(artistName, songTitle));
    }

}
