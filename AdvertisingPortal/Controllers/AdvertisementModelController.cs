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
        private static Random random = new Random();

        [Authorize]
        public ActionResult Create() {
            ViewBag.categories = db.Categories.ToList();

            return View();
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(AdvertisementCreate advertisement) {
            TryUpdateModel(advertisement);
            ModelState.Remove("ad.Category.ID");
            ModelState.Remove("ad.Category.Name");
            if (ModelState.IsValid) {
                CategoryModel cat = db.Categories.Where(i => i.ID == advertisement.Ad.Category.ID).Single();

                IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
                UserModel userInfo = db.Users.Where(s => s.ID == user.Id).First();

                advertisement.Ad.Category = cat;

                advertisement.Ad.AddTime = DateTime.Now;
                advertisement.Ad.Active = true;
                advertisement.Ad.User = userInfo;
                
                string _FileName = Path.GetFileName(advertisement.AttachImage.FileName);
                string dirName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + RandomString(8) + Path.GetExtension(_FileName);
                string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), dirName);
                advertisement.AttachImage.SaveAs(_path);
                
                db.Advertisements.Add(advertisement.Ad);
                db.Files.Add(advertisement.Ad.Files);
                db.SaveChanges();
                advertisement.Ad.Files = new FileModel { Path = dirName, AddTime = DateTime.Now};

                return RedirectToAction("UserDetails", "Home");
            }
            ViewBag.categories = db.Categories.ToList();
            return View(advertisement);
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

        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}