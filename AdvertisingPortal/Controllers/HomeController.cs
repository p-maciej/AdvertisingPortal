using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingPortal.Controllers {
    public class HomeController : Controller {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();
        private static ApplicationDbContext appdb = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appdb));

        public ActionResult Index() {
            var advertisements = db.Advertisements.Where(a => a.Active == true).Include(a => a.Files).OrderByDescending(b => b.AddTime).Take(8);

            if(advertisements.Count() > 0) {
                ViewBag.display = true;
            } else {
                ViewBag.display = false;
            }
            return View(advertisements.ToList());
        }

        public ActionResult Contact() {
            return View();
        }

        [Authorize]
        public ActionResult UserDetails() {
            AdvertisementsFavourites package = new AdvertisementsFavourites();

            IQueryable<AdvertisementModel> advertisements;
            IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
            advertisements = db.Advertisements.Where(a => a.User.ID == user.Id).Include(a => a.Files);
            IQueryable<FavouriteModel> favourites = db.Favourites.Where(a => a.User.ID == user.Id).Include(u => u.Advertisement);
            package.Ads = advertisements.ToList();
            package.Favs = favourites.ToList();

            ViewBag.favouritesShow = db.Favourites.Where(a => a.User.ID == user.Id).Count() > 0;
            ViewBag.advertisementsShow = db.Advertisements.Where(a => a.User.ID == user.Id).Count() > 0;
            return View(package);
        }
    }
}