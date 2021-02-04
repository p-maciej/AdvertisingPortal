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
            List<SelectListItem> cat1 = new List<SelectListItem>();

            foreach (CategoryModel item in db.Categories.ToList()) {
                cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString()});
            }

            ViewBag.categories = cat1;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title, Content, Price, ToNegotiate")] AdvertisementModel advertisement, int Category_ID) {
            if (ModelState.IsValid) {
                CategoryModel cat = db.Categories.Where(i => i.ID == Category_ID).Single();
                advertisement.Category = cat;

                advertisement.AddTime = DateTime.Now;
                advertisement.Active = true;
                db.Advertisements.Add(advertisement);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                AdvertisementModel ad = db.Advertisements.AsNoTracking().Single(x => x.ID == advertisement.ID);


                CategoryModel cat = db.Categories.Where(i => i.ID == Category_ID).Single();
                advertisement.Category = cat;

                advertisement.AddTime = ad.AddTime;
                db.Entry(advertisement).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
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