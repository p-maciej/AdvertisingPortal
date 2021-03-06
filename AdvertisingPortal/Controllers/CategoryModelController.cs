using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingPortal.Controllers
{
    public class CategoryModelController : Controller {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();
        public ActionResult Index(int? id) {
            IQueryable<CategoryModel> categories;

            if (id != null) {
                categories = db.Categories.Where(c => c.Parent == id);
            } else {
                categories = db.Categories.Where(c => c.Parent == null);
            }

            var advertisements = db.Advertisements.Where(a => a.Active == true && a.Category.ID == id).Include(a => a.Files).OrderByDescending(b => b.AddTime);

            if (advertisements.Count() > 0) {
                ViewBag.display = true;
            }
            else {
                ViewBag.display = false;
            }

            return View(new CategoryAdvertisementsModel(categories.ToList(), advertisements.ToList()));
        }

        public ActionResult AdminView() {
            DbSet<CategoryModel> categories = db.Categories;
            return View(categories.ToList());
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create() {
            List<SelectListItem> cat = new List<SelectListItem>();

            foreach (CategoryModel item in db.Categories.ToList()) {
                cat.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString() });
            }

            ViewBag.categories = cat;

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create([Bind(Include = "Name, Parent")] CategoryModel category) {
            if (ModelState.IsValid) {
                db.Categories.Add(category);
                db.SaveChanges();

                return RedirectToAction("AdminView");
            }
            return View(category);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id) {
            CategoryModel cat = db.Categories.Where(s => s.ID == id).FirstOrDefault();

            List<SelectListItem> cat1 = new List<SelectListItem>();

            foreach (CategoryModel item in db.Categories.ToList()) {
                cat1.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString(), Selected = item.Parent == item.ID ? true : false });
            }

            ViewBag.categories = cat1;

            return View(cat);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(CategoryModel category) {
            if (ModelState.IsValid) {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(category);       
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id) {
            if (id == 0) {
                ViewBag.Message = String.Format("Category don't exist.");
                return View(new CategoryModel());
            }

            CategoryModel category = db.Categories.Find(id);

            if (category == null) {
                return HttpNotFound();
            }
            return View(category);
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) {
            if (ModelState.IsValid) {
                CategoryModel category = db.Categories.Find(id);
                try {
                    var userAds = db.Advertisements.Where(u => u.Category.ID == category.ID).Include(s => s.Favourites).ToList();

                    foreach (var ad in userAds) {
                        foreach (var fav in ad.Favourites.ToList()) {
                            db.Favourites.Remove(fav);
                        }

                        string strPhysicalFolder = Server.MapPath("~/UploadedFiles/");

                        string strFileFullPath = strPhysicalFolder + ad.Files.Path;
                        if (System.IO.File.Exists(strFileFullPath)) {
                            System.IO.File.Delete(strFileFullPath);
                        }

                        db.Files.Remove(ad.Files);

                        db.Advertisements.Remove(ad);
                    }

                    db.Categories.Remove(category);
                    db.SaveChanges();
                } catch {
                    ViewBag.Message = String.Format("Cannot delete this category.");
                    return View(category);
                }
            }
            return RedirectToAction("AdminView");
        }
    }
}