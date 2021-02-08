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
        public ActionResult Index() {
            DbSet<CategoryModel> categories = db.Categories;
            return View(categories.ToList());
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

                return RedirectToAction("Index");
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
                    db.Categories.Remove(category);
                    db.SaveChanges();
                } catch {
                    ViewBag.Message = String.Format("Cannot delete this category.");
                    return View(category);
                }
            }
            return RedirectToAction("Index");
        }
    }
}