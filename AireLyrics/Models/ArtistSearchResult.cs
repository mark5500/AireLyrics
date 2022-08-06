namespace AireLyrics.Models
{
    public class ArtistSearchResult
    {
        public int Count { get; set; }
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }
}