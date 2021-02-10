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
            ModelState.Remove("Ad.Category.ID");
            ModelState.Remove("Ad.Category.Name");
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

                advertisement.Ad.Files = new FileModel { Path = dirName, AddTime = DateTime.Now };
                db.Advertisements.Add(advertisement.Ad);
                db.Files.Add(advertisement.Ad.Files);
                db.SaveChanges();
                

                return RedirectToAction("UserDetails", "Home");
            }
            ViewBag.categories = db.Categories.ToList();
            return View(advertisement);
        }

        [Authorize]
        public ActionResult Edit(int id) {
            AdvertisementCreate advertisement = new AdvertisementCreate();
            AdvertisementModel ad = db.Advertisements.Where(s => s.ID == id).Include(s => s.Category).FirstOrDefault();

            IdentityUser user = appdb.Users.Where(s => s.UserName == User.Identity.Name).First();

            if (user.Id == ad.User.ID || userManager.IsInRole(user.Id, "admin") || userManager.IsInRole(user.Id, "moderator")) {
                List<SelectListItem> cat1 = new List<SelectListItem>();

                foreach (CategoryModel item in db.Categories.ToList()) {
                    cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString(), Selected = (ad.Category != null && ad.Category.ID == item.ID ? true : false) });
                }
                ViewBag.categories = cat1;

                advertisement.Ad = ad;
                return View(advertisement);
            } else {
                return RedirectToAction("AccessDanied", "Errors");
            }
           
        }

        public ActionResult Details(int id) {
            AdvertisementModel ad = db.Advertisements.Where(s => s.ID == id).Include(s => s.Category).Include(a => a.Files).Include(u => u.User).FirstOrDefault();

            IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
            int check = db.Favourites.Where(f => f.Advertisement.ID == id && f.User.ID == user.Id).Count();

            ViewBag.AddedToFavourites = check > 0;
            return View(ad);
        }

        [HttpPost]
        public ActionResult Edit(AdvertisementCreate advertisement) {
            TryUpdateModel(advertisement);
            ModelState.Remove("Ad.Category.ID");
            ModelState.Remove("Ad.Category.Name");
            ModelState.Remove("AttachImage");
            if (ModelState.IsValid) {
                AdvertisementModel ad = db.Advertisements.Find(advertisement.Ad.ID);

                CategoryModel cat = db.Categories.First(i => i.ID == advertisement.Ad.Category.ID);

                ad.Price = advertisement.Ad.Price;
                ad.Title = advertisement.Ad.Title;
                ad.Content = advertisement.Ad.Content;
                ad.ToNegotiate = advertisement.Ad.ToNegotiate;

                ad.Category = cat;


                if (advertisement.AttachImage != null) {
                    FileModel file = db.Files.First(i => i.ID == ad.Files.ID);
                    string strPhysicalFolder = Server.MapPath("~/UploadedFiles/");

                    string strFileFullPath = strPhysicalFolder + file.Path;
                    if (System.IO.File.Exists(strFileFullPath)) {
                        System.IO.File.Delete(strFileFullPath);
                    }

                    string _FileName = Path.GetFileName(advertisement.AttachImage.FileName);
                    string dirName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + RandomString(8) + Path.GetExtension(_FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), dirName);
                    advertisement.AttachImage.SaveAs(_path);

                    advertisement.Ad.Files = new FileModel { Path = dirName, AddTime = DateTime.Now };
                    ad.Files = advertisement.Ad.Files;
                    db.Files.Remove(file);
                    db.Files.Add(advertisement.Ad.Files);
                }
                
                ad.AddTime = ad.AddTime;
                db.Entry(ad).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("UserDetails", "Home");
            }

            List<SelectListItem> cat1 = new List<SelectListItem>();
            foreach (CategoryModel item in db.Categories.ToList())
                cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString(), Selected = (advertisement.Ad.Category != null && advertisement.Ad.Category.ID == item.ID ? true : false) });
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
                IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
                UserModel userInfo = db.Users.Where(s => s.ID == user.Id).First();

                AdvertisementModel advertisement = db.Advertisements.Find(id);

                if (advertisement.User.ID == userInfo.ID || userManager.IsInRole(user.Id, "admin") || userManager.IsInRole(user.Id, "moderator")) {
                    try {
                        
                        string strPhysicalFolder = Server.MapPath("~/UploadedFiles/");

                        string strFileFullPath = strPhysicalFolder + advertisement.Files.Path;
                        if (System.IO.File.Exists(strFileFullPath)) {
                            System.IO.File.Delete(strFileFullPath);
                        }

                        db.Files.Remove(advertisement.Files);

                        db.Advertisements.Remove(advertisement);
                        db.SaveChanges();
                    }
                    catch {
                        ViewBag.Message = String.Format("Cannot delete this advertisement.");
                        return View(advertisement);
                    }
                } else {
                    return RedirectToAction("AccessDanied", "Errors");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}