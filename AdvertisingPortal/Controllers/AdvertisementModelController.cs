using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingPortal.Controllers
{
    public class AdvertisementModelController : Controller {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();

        public ActionResult Index() {
            DbSet<AdvertisementModel> advertisements = db.Advertisements;
            return View(advertisements.ToList());
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Title, Content, Price, ToNegotiate")] AdvertisementModel advertisement) {
            if (ModelState.IsValid) {
                advertisement.AddTime = DateTime.Now;
                advertisement.Active = true;
                db.Advertisements.Add(advertisement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(advertisement);
        }
    }
}