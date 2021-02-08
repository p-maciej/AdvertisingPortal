using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
            var advertisements = db.Advertisements.Where(a => a.Active == true).OrderByDescending(b => b.AddTime).Take(30);

            if(advertisements.Count() > 0) {
                ViewBag.display = true;
            } else {
                ViewBag.display = false;
            }
            return View(advertisements.ToList());
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult UserDetails() {
            IQueryable<AdvertisementModel> advertisements;
            IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
            advertisements = db.Advertisements.Where(a => a.User.ID == user.Id);

            return View(advertisements.ToList());
        }
    }
}