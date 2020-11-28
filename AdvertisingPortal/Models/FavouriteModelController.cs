using AdvertisingPortal.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingPortal.Models
{
    public class FavouriteModelController : Controller
    {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();
        public ActionResult Index() {
            DbSet<FavouriteModel> favourites = db.Favourites;
            return View(favourites.ToList());
        }
    }
}