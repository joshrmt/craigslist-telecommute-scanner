using System;
using System.Linq;
using System.Web.Mvc;
using CraigslistScanner.Data;

namespace CLWebViewRole.Controllers {
    public class HomeController : Controller {
        public ActionResult Show() {
            using (var database = new CraigslistDatabase()) {
                var date = DateTime.Now.Subtract(TimeSpan.FromDays(14));

                var posts = database.Jobs.Where(each => each.Date > date).ToList();

                return View(posts);
            }
        }

        public ActionResult Search(String term) {
            using (var database = new CraigslistDatabase()) {
                var date = DateTime.Now.Subtract(TimeSpan.FromDays(14));

                var posts = database.Jobs.Where(each => each.Date > date && each.Body.Contains(term)).ToList();

                return View("Show", posts);
            }
        }
    }
}
