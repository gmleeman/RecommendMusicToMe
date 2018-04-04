using Newtonsoft.Json;
using RecommendMusicToMe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace RecommendMusicToMe
{
    public class SpotifyServices
    {
        public string AccessToken { get; set; } = "";

        private Recommendations.SpotifyRecommendationSeedGenres _AllGenres = new Recommendations.SpotifyRecommendationSeedGenres();

        public List<string> genres
        {
            get
            {
                if (_AllGenres.genres.Count == 0)
                    LoadSeedGenres();
                return _AllGenres.genres;
            }
        }




        private string GetSearchUri(string searchquery, string type, int limit, int offset)
        {
            string converted = HttpUtility.HtmlEncode(searchquery);
            var endpoint = new StringBuilder("https://api.spotify.com/v1/");
            endpoint.Append($"search?q={converted}");
            endpoint.Append($"&type={type}");
            endpoint.Append($"&market=from_token");
            endpoint.Append($"&limit={limit}");
            endpoint.Append($"&offset={offset}");
            return endpoint.ToString();
        }

        private string GetRecommendationUri(SpotifyRecSearch criteria)
        {
            //string converted = HttpUtility.HtmlEncode(searchquery);
            var endpoint = new StringBuilder("https://api.spotify.com/v1/recommendations");
            endpoint.Append($"?seed_artists={criteria.seed_artists}");
            endpoint.Append($"&seed_tracks={criteria.seed_tracks}");
            endpoint.Append($"&seed_genres={criteria.seed_genres}");
            endpoint.Append($"&limit={criteria.limit}");
            endpoint.Append($"&market={criteria.market}");

            endpoint.Append($"&target_danceability={criteria.target_danceability}");
            endpoint.Append($"&target_energy={criteria.target_energy}");
            endpoint.Append($"&target_popularity={criteria.target_popularity}");
            if (criteria.target_tempo > 0)
            {
                endpoint.Append($"&target_tempo={criteria.target_tempo}");
            }

            endpoint.Append($"&target_valence={criteria.target_valence}");

            return endpoint.ToString();

        }

        /// <summary>
        /// Search for results in Spotify
        /// </summary>
        /// <param name="searchquery">Do not encode this string, will be done automatically</param>
        /// <param name="type"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public SpotifySearchResults GetSearchResults(string searchquery, string type, int limit, int offset)
        {
            if (offset < 0) offset = 0;
            if (limit < 0) limit = 0;
            string endpoint = GetSearchUri(searchquery, type, limit, offset);
            SpotifySearchResults spotResults = GetFromSpotify<SpotifySearchResults>(endpoint, AccessToken);
            return spotResults;

        }

        public SpotifyUserInfo GetUserProfile()
        {
            string endpoint = "https://api.spotify.com/v1/me";
            SpotifyUserInfo spotUser = GetFromSpotify<SpotifyUserInfo>(endpoint, AccessToken);
            return spotUser;

        }

        /// <summary>
        /// from https://developer.spotify.com/web-api/get-recommendations/
        /// </summary>
        /// <returns></returns>
        public void LoadSeedGenres()
        {
            string endpoint = "https://api.spotify.com/v1/recommendations/available-genre-seeds";

            if (_AllGenres.genres.Count == 0)
                _AllGenres = GetFromSpotify<Recommendations.SpotifyRecommendationSeedGenres>(endpoint, AccessToken);

        }


        public Recommendations.SpotifyRecommendations GetRecommendations(SpotifyRecSearch criteria)
        {
            string endpoint = GetRecommendationUri(criteria);

            Recommendations.SpotifyRecommendations spotRecs = GetFromSpotify<Recommendations.SpotifyRecommendations>(endpoint, AccessToken);
            return spotRecs;
        }

        public SpotifyPlaylists GetPlaylists(string userid)
        {
            string endpoint = $"https://api.spotify.com/v1/users/{userid}/playlists";
            SpotifyPlaylists spotPlays = GetFromSpotify<SpotifyPlaylists>(endpoint, AccessToken);
            return spotPlays;
        }

        public TrackObject GetTrack(string trackid)
        {
            string endpoint = $"https://api.spotify.com/v1/tracks/{trackid}";
            TrackObject spotTrack = GetFromSpotify<TrackObject>(endpoint, AccessToken);
            return spotTrack;
        }

        public T GetFromSpotify<T>(string url_endpoint, string acesstoken)
        {
            try
            {
                WebRequest wreq = WebRequest.Create(url_endpoint);
                wreq.Method = "GET";
                wreq.Headers.Set("Authorization", "Bearer " + acesstoken);
                wreq.ContentType = "application/json; charset=utf-8";
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                T typeis = default(T);

                using (WebResponse response = wreq.GetResponse())
                {
                    using (Stream data = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            string responseServer = reader.ReadToEnd();
                            typeis = JsonConvert.DeserializeObject<T>(responseServer, settings);
                        }
                    }
                }
                return typeis;
            }
            catch (WebException ex)
            {
                return default(T);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}