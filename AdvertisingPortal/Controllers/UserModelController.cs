using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdvertisingPortal.Controllers {
    
    public class UserModelController : Controller {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();
        private ApplicationDbContext appdb = new ApplicationDbContext();

        public ActionResult Index() {
            List<CompleteUserModel> list = new List<CompleteUserModel>();
            var users = appdb.Users;
            foreach (IdentityUser user in users) {
                UserModel userInfo = db.Users.Find(user.Id);

                list.Add(new CompleteUserModel(user, userInfo));
            }
            return View(list);
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "FirstName, LastName, PhoneNumber, City")] UserModel userModel) { 
            if(ModelState.IsValid) {
                db.Users.Add(userModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userModel);
        }

        public ActionResult Edit(string id) {
            UserModel usr = db.Users.Where(s => s.ID == id).FirstOrDefault();

            return View(usr);
        }

        [HttpPost]
        public ActionResult Edit(UserModel user) {
            if (ModelState.IsValid) {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int? id) {
            if (id == 0) {
                ViewBag.Message = String.Format("Category don't exist.");
                return View(new UserModel());
            }

            UserModel user = db.Users.Find(id);

            if (user == null) {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) {
            if (ModelState.IsValid) {
                UserModel user = db.Users.Find(id);
                try {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                catch {
                    ViewBag.Message = String.Format("Cannot delete this category.");
                    return View(user);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(string id) {
            UserModel usr = db.Users.Where(s => s.ID == id).FirstOrDefault();

            return View(usr);
        }
    }
}