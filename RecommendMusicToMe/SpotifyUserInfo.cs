using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecommendMusicToMe
{
    public class SpotifyUserInfo
    {
        [JsonProperty("id")]
        public string UserID { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

    }
}