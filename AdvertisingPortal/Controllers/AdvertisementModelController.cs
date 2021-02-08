using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        private static ApplicationDbContext appdb = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appdb));

        [Authorize]
        public ActionResult Create() {
            List<SelectListItem> cat1 = new List<SelectListItem>();

            foreach (CategoryModel item in db.Categories.ToList()) {
                cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString()});
            }

            ViewBag.categories = cat1;

            return View();
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title, Content, Price, ToNegotiate")] AdvertisementModel advertisement, int Category_ID) {
            if (ModelState.IsValid) {
                CategoryModel cat = db.Categories.Where(i => i.ID == Category_ID).Single();

                IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
                UserModel userInfo = db.Users.Where(s => s.ID == user.Id).First();

                advertisement.Category = cat;

                advertisement.AddTime = DateTime.Now;
                advertisement.Active = true;
                advertisement.User = userInfo;

                db.Advertisements.Add(advertisement);
                db.SaveChanges();
                return RedirectToAction("UserDetails", "Home");
            }
            return View(advertisement);
        }

        public ActionResult Edit(int id) {
            AdvertisementModel ad = db.Advertisements.Where(s => s.ID == id).Include(s => s.Category).FirstOrDefault();

            List<SelectListItem> cat1 = new List<SelectListItem>();

            foreach (CategoryModel item in db.Categories.ToList()) {
                cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString(), Selected = (ad.Category != null && ad.Category.ID == item.ID ? true : false) });
            }

            ViewBag.categories = cat1;

            return View(ad);
        }

        [HttpPost]
        public ActionResult Edit(AdvertisementModel advertisement, int Category_ID) {
            if (ModelState.IsValid) {
                AdvertisementModel ad = db.Advertisements.First(x => x.ID == advertisement.ID);

                CategoryModel cat = db.Categories.First(i => i.ID == Category_ID);
                advertisement.Category = cat;

                advertisement.AddTime = ad.AddTime;
                db.Entry(advertisement).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("UserDetails", "Home");
            }
            return View(advertisement);
        }

        public ActionResult Delete(int? id) {
            if (id == 0) {
                ViewBag.Message = String.Format("Advertisement doesn't exist.");
                return View(new AdvertisementModel());
            }

            AdvertisementModel advertisement = db.Advertisements.Find(id);

            if (advertisement == null) {
                return HttpNotFound();
            }
            return View(advertisement);
        }

        [ActionName("Deactivate")]
        public ActionResult Deactivate(int id) {
            AdvertisementModel advertisement = db.Advertisements.Find(id);

            advertisement.CloseTime = DateTime.Now;
            advertisement.Active = false;

            db.SaveChanges();

            return RedirectToAction("UserDetails", "Home");
        }

        [ActionName("Activate")]
        public ActionResult Activate(int id) {
            AdvertisementModel advertisement = db.Advertisements.Find(id);

            advertisement.CloseTime = null;
            advertisement.Active = true;

            db.SaveChanges();

            return RedirectToAction("UserDetails", "Home");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) {
            if (ModelState.IsValid) {
                AdvertisementModel advertisement = db.Advertisements.Find(id);
                try {
                    db.Advertisements.Remove(advertisement);
                    db.SaveChanges();
                }
                catch {
                    ViewBag.Message = String.Format("Cannot delete this advertisement.");
                    return View(advertisement);
                }
            }
            return RedirectToAction("Index");
        }
    }
}