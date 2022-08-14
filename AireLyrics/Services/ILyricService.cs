using AireLyrics.Models;

namespace AireLyrics.Services;

public interface ILyricService
{
    Task<SearchLyricsResponse> SearchLyrics(string artistName, string songTitle);
}
