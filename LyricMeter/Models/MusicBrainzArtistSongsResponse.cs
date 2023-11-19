namespace LyricMeter.Models
{
    public class MusicBrainzArtistSongsResponse
    {
        public DateTime? Created { get; set; }
        public int Count { get; set; }
        public int Offset { get; set; }
        public List<Work>? Works { get; set; }
    }
}
