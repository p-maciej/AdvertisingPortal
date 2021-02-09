using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            ViewBag.categories = db.Categories.ToList();

            return View();
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(AdvertisementModel ad) {
            TryUpdateModel(ad);
            ModelState.Remove("Category.ID");
            ModelState.Remove("Category.Name");
            if (ModelState.IsValid) {
                CategoryModel cat = db.Categories.Where(i => i.ID == ad.Category.ID).Single();

                IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
                UserModel userInfo = db.Users.Where(s => s.ID == user.Id).First();

                ad.Category = cat;

                ad.AddTime = DateTime.Now;
                ad.Active = true;
                ad.User = userInfo;

                //string _FileName = Path.GetFileName(ad.file.FileName);
                //string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                //ad.file.SaveAs(_path);

                db.Advertisements.Add(ad);
                db.SaveChanges();
                return RedirectToAction("UserDetails", "Home");
            }
            ViewBag.categories = db.Categories.ToList();
            return View(ad);
        }

        [Authorize]
        public ActionResult Edit(int id) {
            AdvertisementModel ad = db.Advertisements.Where(s => s.ID == id).Include(s => s.Category).FirstOrDefault();

            IdentityUser user = appdb.Users.Where(s => s.UserName == User.Identity.Name).First();

            if (user.Id == ad.User.ID || userManager.IsInRole(user.Id, "admin") || userManager.IsInRole(user.Id, "moderator")) {
                List<SelectListItem> cat1 = new List<SelectListItem>();

                foreach (CategoryModel item in db.Categories.ToList()) {
                    cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString(), Selected = (ad.Category != null && ad.Category.ID == item.ID ? true : false) });
                }
                ViewBag.categories = cat1;
                return View(ad);
            } else {
                return RedirectToAction("AccessDanied", "Errors");
            }
           
        }

        public ActionResult Details(int id) {
            AdvertisementModel ad = db.Advertisements.Where(s => s.ID == id).Include(s => s.Category).FirstOrDefault();

            return View(ad);
        }

        [HttpPost]
        public ActionResult Edit(AdvertisementModel advertisement) {
            TryUpdateModel(advertisement);
            ModelState.Remove("Category.ID");
            ModelState.Remove("Category.Name");
            if (ModelState.IsValid) {
                AdvertisementModel ad = db.Advertisements.Find(advertisement.ID);

                CategoryModel cat = db.Categories.First(i => i.ID == advertisement.Category.ID);

                ad.Price = advertisement.Price;
                ad.Title = advertisement.Title;
                ad.Content = advertisement.Content;
                ad.ToNegotiate = advertisement.ToNegotiate;

                ad.Category = cat;


                ad.AddTime = ad.AddTime;
                db.Entry(ad).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("UserDetails", "Home");
            }

            List<SelectListItem> cat1 = new List<SelectListItem>();
            foreach (CategoryModel item in db.Categories.ToList())
                cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString(), Selected = (advertisement.Category != null && advertisement.Category.ID == item.ID ? true : false) });
            ViewBag.categories = cat1;
            return View(advertisement);
        }

        [Authorize]
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

        [Authorize]
        [ActionName("Deactivate")]
        public ActionResult Deactivate(int id) {
            AdvertisementModel advertisement = db.Advertisements.Find(id);

            advertisement.CloseTime = DateTime.Now;
            advertisement.Active = false;

            db.SaveChanges();

            return RedirectToAction("UserDetails", "Home");
        }

        [Authorize]
        [ActionName("Activate")]
        public ActionResult Activate(int id) {
            AdvertisementModel advertisement = db.Advertisements.Find(id);

            advertisement.CloseTime = null;
            advertisement.Active = true;

            db.SaveChanges();

            return RedirectToAction("UserDetails", "Home");
        }

        [Authorize]
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