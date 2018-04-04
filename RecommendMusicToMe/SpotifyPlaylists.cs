using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecommendMusicToMe
{
    public class SpotifyPlaylists
    {
        [JsonProperty("items")]
        public List<Playlist> Items { get; set; }


    }

    public class Playlist
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("owner")]
        public PublicProfile Owner { get; set; }

    }

    public class PublicProfile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

    }

}