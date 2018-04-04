using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.Caching;
using RecommendMusicToMe.Models;

namespace RecommendMusicToMe.Controllers
{
    public class SpotifyController : Controller
    {
        static SpotifyServices spotService = new SpotifyServices();


        private ObjectCache cache = MemoryCache.Default;

        private SearchCriteriaModel GetCriteriaClass()
        {
            SearchCriteriaModel criteriaSpotify;
            if (cache["SearchCriteriaModel"] == null)
            {
                criteriaSpotify = new SearchCriteriaModel();
                cache["SearchCriteriaModel"] = criteriaSpotify;
            }
            else
            {
                criteriaSpotify = (SearchCriteriaModel)cache["SearchCriteriaModel"];
            }
            return criteriaSpotify;
        }

        // GET: Spotify
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddFavouriteTrack(SearchCriteriaModel criteriaSpotify, string addtrackid, string addtrackname)
        {
            if (addtrackid != null && addtrackname != null)
                if (addtrackid != "" && addtrackname != "")
                {
                    if (!criteriaSpotify.FavouriteTracks.Exists(x => x.TrackID == addtrackid))
                    {
                        var newTrack = new FavouriteTrackItem();
                        newTrack.TrackID = addtrackid;
                        newTrack.TrackName = addtrackname;
                        criteriaSpotify.FavouriteTracks.Add(newTrack);
                    }
                }
            cache["SearchCriteriaModel"] = criteriaSpotify; // store in cache

            return RedirectToAction("ListSearchResults", new { searchText = criteriaSpotify.SearchText });
        }


        public ActionResult ListSearchResults(string searchText)
        {
            var criteriaSpotify = GetCriteriaClass();
            if (searchText != "")
                criteriaSpotify.SearchText = searchText;
            return ListSearchResults(criteriaSpotify);
        }

        public ActionResult ListSearchResultsNextPage()
        {
            var criteriaSpotify = GetCriteriaClass();
            criteriaSpotify.PageNumber++;
            cache["SearchCriteriaModel"] = criteriaSpotify; // store in cache
            return ListSearchResults(criteriaSpotify);
        }


        [HttpPost]
        public ActionResult ListSearchResults(SearchCriteriaModel criteriaSpotify)
        {
            if (criteriaSpotify == null)
            {
                criteriaSpotify = GetCriteriaClass();
            }
            spotService.AccessToken = (string)cache["access_token"];
            //

            if (ModelState.IsValid)
            {
                // save?
                //return RedirectToAction("Index");
                var searchResults = spotService.GetSearchResults(criteriaSpotify.SearchText, criteriaSpotify.TypeParamString(), criteriaSpotify.PageSize, (criteriaSpotify.PageNumber - 1) * criteriaSpotify.PageSize);


                criteriaSpotify.IsNextPage = false;
                if (searchResults != null)
                {
                    if (searchResults.albums == null ? false : searchResults.albums.items.Count == criteriaSpotify.PageSize)
                        criteriaSpotify.IsNextPage = true;
                    if (searchResults.artists == null ? false : searchResults.artists.items.Count == criteriaSpotify.PageSize)
                        criteriaSpotify.IsNextPage = true;
                    if (searchResults.playlists == null ? false : searchResults.playlists.items.Count == criteriaSpotify.PageSize)
                        criteriaSpotify.IsNextPage = true;
                    if (searchResults.tracks == null ? false : searchResults.tracks.items.Count == criteriaSpotify.PageSize)
                        criteriaSpotify.IsNextPage = true;
                }

                ViewBag.SearchSpotify = searchResults;
            }

            return View(criteriaSpotify);
        }


        public ActionResult ListSearchResults_Spent(string searchNow, bool? ShowAlbum, bool? ShowPlaylist, bool? ShowArtist, bool? ShowTrack, int? ShowPage)
        {
            spotService.AccessToken = (string)cache["access_token"];
            var criteriaSpotify = GetCriteriaClass();

            criteriaSpotify.SearchText = searchNow;
            if (ShowAlbum.HasValue)
                criteriaSpotify.ListAlbums = ShowAlbum.Value;
            if (ShowPlaylist.HasValue)
                criteriaSpotify.ListPlaylist = ShowPlaylist.Value;
            if (ShowArtist.HasValue)
                criteriaSpotify.ListArtists = ShowArtist.Value;
            if (ShowTrack.HasValue)
                criteriaSpotify.ListTrack = ShowTrack.Value;

            if (ShowPage.HasValue)
                criteriaSpotify.PageNumber = ShowPage.Value;

            var searchResults = spotService.GetSearchResults(searchNow, criteriaSpotify.TypeParamString(), criteriaSpotify.PageSize, (criteriaSpotify.PageNumber - 1) * criteriaSpotify.PageSize);


            criteriaSpotify.IsNextPage = false;
            if (searchResults != null)
            {
                if (searchResults.albums == null ? false : searchResults.albums.items.Count == criteriaSpotify.PageSize)
                    criteriaSpotify.IsNextPage = true;
                if (searchResults.artists == null ? false : searchResults.artists.items.Count == criteriaSpotify.PageSize)
                    criteriaSpotify.IsNextPage = true;
                if (searchResults.playlists == null ? false : searchResults.playlists.items.Count == criteriaSpotify.PageSize)
                    criteriaSpotify.IsNextPage = true;
                if (searchResults.tracks == null ? false : searchResults.tracks.items.Count == criteriaSpotify.PageSize)
                    criteriaSpotify.IsNextPage = true;
            }

            ViewBag.SearchSpotify = searchResults;
            return View(criteriaSpotify);
        }


        public ActionResult FirstSearchResults()
        {
            string searchText = "blues";

            spotService.AccessToken = (string)cache["access_token"];

            // LOAD Recommendation stuff
            spotService.LoadSeedGenres();

            ViewBag.SearchQuery = searchText;
            ViewBag.SearchSpotify = spotService.GetSearchResults(searchText, "album,artist,playlist,track", 40, 0);

            return View();
        }

        public ActionResult callback(string access_token, string token_type, string expires_in, string state)
        {
            if (string.IsNullOrEmpty(access_token))
                return View();

            cache["access_token"] = access_token;
            cache["token_type"] = token_type;
            cache["expires_in"] = expires_in;
            cache["state"] = state;

            spotService.AccessToken = access_token;

            SpotifyUserInfo spotUser = spotService.GetUserProfile();

            SpotifyPlaylists spotPlays = spotService.GetPlaylists(spotUser.UserID);

            ViewBag.Playlists = spotPlays;

            return View();
        }

        [HttpPost]
        public ActionResult Recommendations(SearchCriteriaModel criteriaSpotify)
        {
            spotService.AccessToken = (string)cache["access_token"];

            var newRecSearch = new SpotifyRecSearch();

            newRecSearch.seed_tracks = string.Join(",", criteriaSpotify.FavouriteTracks.Select(x => x.TrackID).ToList());
            newRecSearch.seed_artists = "";
            newRecSearch.seed_genres = criteriaSpotify.seed_genres;
            newRecSearch.market = "SE";
            newRecSearch.limit = 50;

            newRecSearch.target_popularity = criteriaSpotify.target_popularity;
            newRecSearch.target_danceability = criteriaSpotify.target_danceability;
            newRecSearch.target_energy = criteriaSpotify.target_energy;
            newRecSearch.target_tempo = criteriaSpotify.target_tempo;
            newRecSearch.target_valence = criteriaSpotify.target_valence;

            var recResults = spotService.GetRecommendations(newRecSearch);

            ViewBag.Recommendations = recResults;
            ViewBag.GenreList = spotService.genres;

            return View(criteriaSpotify);
        }

        public ActionResult GetRecommendations(string someQuery)
        {
            spotService.AccessToken = (string)cache["access_token"];
            var criteriaSpotify = GetCriteriaClass();
            var newRecSearch = new SpotifyRecSearch();

            newRecSearch.seed_tracks = string.Join(",", criteriaSpotify.FavouriteTracks.Select(x => x.TrackID).ToList());
            newRecSearch.seed_artists = "";
            newRecSearch.seed_genres = criteriaSpotify.seed_genres;
            newRecSearch.market = "SE";
            newRecSearch.limit = 50;

            newRecSearch.target_popularity = criteriaSpotify.target_popularity;
            newRecSearch.target_danceability = criteriaSpotify.target_danceability;
            newRecSearch.target_energy = criteriaSpotify.target_energy;
            newRecSearch.target_tempo = criteriaSpotify.target_tempo;
            newRecSearch.target_valence = criteriaSpotify.target_valence;

            var recResults = spotService.GetRecommendations(newRecSearch);

            ViewBag.Recommendations = recResults;

            ViewBag.GenreList = spotService.genres;

            return View("Recommendations", criteriaSpotify);
        }

    }
}