namespace LyricMeter.Models
{
    public class LyricMeterArtistSongsResponse
    {
        public DateTime Created { get; set; }
        public string Artist { get; set; } = default!;
        public string ArtistId { get; set; } = default!;
        public List<Work> Songs { get; set; } = default!;
    }
}