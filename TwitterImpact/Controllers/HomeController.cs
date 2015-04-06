using System.Threading.Tasks;
using System.Web.Mvc;
using LinqToTwitter;
using TwitterImpact.Core;

namespace TwitterImpact.Controllers
{
    public class HomeController : Controller
    {
        private const int NumUsersToShow = 10; //(todo) Make this configurable

        [ActionName("Index"), Authorize]
        public async Task<ActionResult> IndexAsync()
        {
            if(!new SessionStateCredentialStore().HasAllCredentials())
            {
                return RedirectToAction("Index", "OAuth");
            }

            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore()
            };

            var repo = new TwitterRepository(auth);

            var tweets = repo.ListTopUsers(NumUsersToShow);

            await tweets;

            return View(tweets.Result);
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