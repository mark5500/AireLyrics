using System.Text.Json.Serialization;

namespace AireLyrics.Models;

public class GetWorksResponse
{
    [JsonPropertyName("work-count")]
    public int WorkCount { get; set; }
    public List<Work> Works { get; set; } = new List<Work>();
}