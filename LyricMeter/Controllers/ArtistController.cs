using LyricMeter.Factories;
using LyricMeter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LyricMeter.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly string _musicBrainzApi = "http://musicbrainz.org/ws/2/";
        private readonly string _fakeLyricApi = "https://api.spacexdata.com/v3/missions/";
        private readonly int _cacheExpirationHours = 72;
        private readonly List<string> _fakeSongIds = new List<string> 
        { 
            "9D1B7E0", 
            "F4F83DE", 
            "F3364BF", 
            "EE86F74", 
            "6C42550", 
            "FE3533D", 
            "593B499", 
            "CE91D46", 
            "2CF444A", 
            "F7709F2"
        };

        public ArtistController(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("LyricMeter", "1.0"));
        }

        private async Task<LyricMeterArtistSongsResponse> GetArtistWorkAsync(string name, string artistId)
        {
            int offset = 0;
            int maxRequests = 15;
            int requestCount = 0;
            var decodedName = Uri.UnescapeDataString(name);
            var parsed = LyricMeterArtistSongsResponseFactory.Create(decodedName, artistId);

            while (requestCount < maxRequests)
            {
                string query = $"work/?query=arid:{artistId}&limit=100&offset={offset}&fmt=json";
                var response = await ExternalApiCall(_musicBrainzApi, query);

                var responseObj = JsonConvert.DeserializeObject<MusicBrainzArtistSongsResponse>(response);

                if (responseObj == null || responseObj.Works == null || !responseObj.Works.Any())
                {
                    break;
                }
                else
                {
                    LyricMeterArtistSongsResponseFactory.Add(responseObj.Works, parsed);
                    offset += 100;
                    requestCount++;
                }

            }

            return parsed;
        }

        private async Task<string> ExternalApiCall(string api, string endpoint, string query = "")
        {
            string url = api + endpoint + (!string.IsNullOrEmpty(query) ? $"{Uri.EscapeDataString(query)}" : "");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private int countWords(string lyric)
        {
            char[] separators = new char[] { ' ', '\n', '\t', ',', '.', ';', ':', '!', '?' };
            return lyric.Split(separators, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        [HttpGet("GetArtistName/{artistName}")]
        public async Task<IActionResult> GetArtistName([FromRoute] string artistName)
        {
            var cacheKey = $"ArtistName_{artistName}";
            if (!_cache.TryGetValue(cacheKey, out string cachedResponse))
            {
                var response = await ExternalApiCall(_musicBrainzApi, "artist/?query=artist:", artistName);
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(_cacheExpirationHours));

                _cache.Set(cacheKey, response, cacheEntryOptions);
                return Ok(response);
            }

            return Ok(cachedResponse);
        }

        [HttpGet("GetAllSongsByArtist/{name}/{artistId}")]
        public async Task<IActionResult> GetAllSongsByArtist([FromRoute] string name, string artistId)
        {
            var cacheKey = $"ArtistSongs_{artistId}";
            if (_cache.TryGetValue(cacheKey, out LyricMeterArtistSongsResponse cachedResponse))
            {
                return Ok(cachedResponse);
            }

            var parsed = await GetArtistWorkAsync(name, artistId);

            if(LyricMeterArtistSongsResponseFactory.GetSongsCount(parsed) == 0)
            {
                return Ok(parsed);
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(72));
            _cache.Set(cacheKey, parsed, cacheEntryOptions);

            return Ok(parsed);
        }

        [HttpGet("GetAllLyricsByArtist/{name}/{artistId}")]
        public async Task<IActionResult> GetAllLyricsByArtist([FromRoute] string name, string artistId)
        {
            var decodedName = Uri.UnescapeDataString(name);
            LyricMeterArtistSongsResponse songs;

            // check cache for lyrics
            var cacheKeyLyrics = $"ArtistLyrics_{artistId}";
            if (_cache.TryGetValue(cacheKeyLyrics, out List<SongLyric> cachedLyrics))
            {
                return Ok(cachedLyrics);
            }

            // get artist's songs from cache or external API
            var cacheKeySongs = $"ArtistSongs_{artistId}";
            if (_cache.TryGetValue(cacheKeySongs, out LyricMeterArtistSongsResponse cachedResponse))
            {
                songs = cachedResponse;
            }
            else
            {
                songs = await GetArtistWorkAsync(name, artistId);
            }

            // get lyrics for each song
            List<SongLyric> lyricsList = new List<SongLyric>();

            foreach (var song in songs.Songs)
            {
                if (!string.IsNullOrEmpty(song.Title))
                {
                    // as we don't have a real lyrics API, we'll use a fake one
                    // we'd use song.Title or song.Id to get the lyrics from a real API
                    // but for now we'll just use a random string from _fakeSongIds
                    var fakeSongId = _fakeSongIds[new Random().Next(0, _fakeSongIds.Count)];
                    var lyricResponse = await ExternalApiCall(_fakeLyricApi, fakeSongId);

                    var lyricObj = JsonConvert.DeserializeObject<FakeLyric>(lyricResponse);
                    if (lyricObj != null)
                    {
                        lyricsList.Add(new SongLyric
                        {
                            Artist = decodedName,
                            ArtistId = artistId,
                            Title = song.Title,
                            Lyrics = lyricObj.Description,
                            WordCount = string.IsNullOrEmpty(lyricObj.Description) ? 0 : countWords(lyricObj.Description)
                        });
                    }
                }
            }

            // save and return lyrics
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(72));
            _cache.Set(cacheKeyLyrics, lyricsList, cacheEntryOptions);

            return Ok(lyricsList);
        }


    }

}
