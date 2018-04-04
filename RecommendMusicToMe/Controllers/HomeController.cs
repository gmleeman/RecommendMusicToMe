using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RecommendMusicToMe.Controllers
{
    public class HomeController : Controller
    {
        private String clientid = "d81190a7ebee43c09effa90bafeffe73";


        private String redir = "http://localhost:36670/Spotify/callback";

        public ActionResult Index()
        {
            ViewBag.AuthUri = GetUri();
            return View();
        }


        private string GetUri()
        {
            StringBuilder builder = new StringBuilder("https://accounts.spotify.com/authorize/?");
            builder.Append("client_id=" + clientid);
            builder.Append("&response_type=token");
            builder.Append("&redirect_uri=" + redir);
            builder.Append("&state=1234567890");
            builder.Append("&scope=user-read-private");
            builder.Append("&show_dialog=true");
            //Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}