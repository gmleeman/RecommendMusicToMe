using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace RecommendMusicToMe.Models
{
    public class FavouriteTrackItem
    {
        public string TrackID { get; set; }
        public string TrackName { get; set; }
    }

    public class SearchCriteriaModel
    {

        public SearchCriteriaModel()
        {
            PageNumber = 1;
            PageSize = 25;
            ListAlbums = true;
            ListArtists = true;
            ListPlaylist = false;
            ListTrack = true;
            //FavouriteTracks.Add(new FavouriteTrackItem { TrackID = "1", TrackName = "Test" });

            target_popularity = 50;
            target_danceability = 0.5F;
            target_energy = 0.5F;
            target_tempo = 0;
            target_valence = 0.5F;
        }

        [Required]
        [Display(Name = "Search text")]
        public string SearchText { get; set; }

        [Display(Name = "Show albums")]
        public bool ListAlbums { get; set; }

        [Display(Name = "Show artists")]
        public bool ListArtists { get; set; }

        [Display(Name = "Show playlists")]
        public bool ListPlaylist { get; set; }

        [Display(Name = "Show tracks")]
        public bool ListTrack { get; set; }

        public List<FavouriteTrackItem> FavouriteTracks { get; set; } = new List<FavouriteTrackItem>();

        [Display(Name = "Danceability 0.0 - 1.0")]
        public float target_danceability { get; set; }

        [Display(Name = "Energy 0.0 - 1.0")]
        public float target_energy { get; set; }

        [Display(Name = "Tempo (beats per minute. 0 - ignore)")]
        public float target_tempo { get; set; }

        [Display(Name = "Valence 0.0 - 1.0")]
        public float target_valence { get; set; }

        [Display(Name = "Popularity 0 - 100")]
        public int target_popularity { get; set; }

        [Display(Name = "Genres, separated by commas")]
        public string seed_genres { get; set; }

        #region Paging variables

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsNextPage { get; set; }

        public int NextPage()
        {
            return PageNumber + 1;
        }

        public int PreviousPage()
        {
            if (PageNumber - 1 < 1)
                return 1;
            return PageNumber - 1;
        }
        #endregion

        public string GetBootStrapColWidth()
        {
            List<string> listTypes = new List<string>();
            if (ListAlbums)
                listTypes.Add("album");
            if (ListArtists)
                listTypes.Add("artist");
            if (ListPlaylist)
                listTypes.Add("playlist");
            if (ListTrack)
                listTypes.Add("track");

            switch (listTypes.Count)
            {
                case 0:
                case 1:
                case 2:
                    return "col-md-6";
                case 3:
                    return "col-md-4";
            }
            return "col-md-3";
        }

        /// <summary>
        /// Convert bool flags to text for the Spotify API call, like
        /// album,artist,playlist,track
        /// </summary>
        /// <returns></returns>
        public string TypeParamString()
        {
            List<string> listTypes = new List<string>();
            if (ListAlbums)
                listTypes.Add("album");
            if (ListArtists)
                listTypes.Add("artist");
            if (ListPlaylist)
                listTypes.Add("playlist");
            if (ListTrack)
                listTypes.Add("track");

            return string.Join(",", listTypes);
        }
    }

}