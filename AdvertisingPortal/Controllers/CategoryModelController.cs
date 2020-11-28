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
    public class CategoryModelController : Controller {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();
        public ActionResult Index() {
            DbSet<CategoryModel> categories = db.Categories;
            return View(categories.ToList());
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Name, Parent")] CategoryModel category) {
            if (ModelState.IsValid) {
                db.Categories.Add(category);
                db.SaveChanges();

                SelectList categories = new SelectList(db.Categories.ToList(), "ID", "Name");
                ViewBag.categories = categories;

                return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}