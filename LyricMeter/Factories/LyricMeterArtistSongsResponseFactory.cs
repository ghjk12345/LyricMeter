using LyricMeter.Models;

namespace LyricMeter.Factories
{
    public class LyricMeterArtistSongsResponseFactory
    {
        public static LyricMeterArtistSongsResponse Create(string artistName, string artistId)
        {
            return new LyricMeterArtistSongsResponse
            {
                Created = DateTime.UtcNow,
                Artist = artistName,
                ArtistId = artistId,
                Songs = new List<Work>()
            };
        }

        public static void Add(List<Work> works, LyricMeterArtistSongsResponse response)
        {
            response.Songs.AddRange(works);
        }

        public static int GetSongsCount(LyricMeterArtistSongsResponse response)
        {
            return response.Songs.Count;
        }
    }
}
