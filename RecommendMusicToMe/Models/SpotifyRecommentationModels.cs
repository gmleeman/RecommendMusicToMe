using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecommendMusicToMe.Models
{
    public class SpotifyRecSearch
    {
        public int limit { get; set; }
        public string market { get; set; }
        public string seed_genres { get; set; }
        public string seed_artists { get; set; }
        public string seed_tracks { get; set; }
        public float target_energy { get; set; }
        public float target_danceability { get; set; }
        public float target_tempo { get; set; }
        public float target_valence { get; set; }
        public int target_popularity { get; set; }

    }
}