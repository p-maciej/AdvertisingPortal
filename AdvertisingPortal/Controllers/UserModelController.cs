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
    [Authorize(Roles = "admin")]    
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

        public ActionResult Edit(string id) {
            IdentityUser user = appdb.Users.Where(s => s.Id == id).First();
            UserModel userInfo = db.Users.Where(s => s.ID == id).First();

            CompleteUserModel cuser = new CompleteUserModel { user = user, userInfo = userInfo };

            ViewBag.roles = new MultiSelectList(appdb.Roles, "Id", "Name", appdb.Roles.Select(c => c.Id).ToArray());

            return View(cuser);
        }

        [HttpPost]
        public ActionResult Edit(CompleteUserModel val) {
            if (ModelState.IsValid) {
                IdentityUser userE = appdb.Users.First(s => s.Id == val.user.Id);
                userE.Email = val.user.Email;
                UserModel userInfo = db.Users.First(s => s.ID == val.user.Id);
                userInfo.FirstName = val.userInfo.FirstName;
                userInfo.LastName = val.userInfo.LastName;
                userInfo.City = val.userInfo.City;
                userInfo.PhoneNumber = val.userInfo.PhoneNumber;

                db.SaveChanges();
                appdb.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(val);
        }

        public ActionResult ConfirmDelete(string id) {
            UserModel user = db.Users.Find(id);
            IdentityUser identity = appdb.Users.Find(id);

            if (user == null) {
                return HttpNotFound();
            }
            return View(new CompleteUserModel(identity, user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id) {
            if (ModelState.IsValid) {
                UserModel user = db.Users.Find(id);
                ApplicationUser identity = appdb.Users.Find(id);
                try {
                    db.Users.Remove(user);
                    appdb.Users.Remove(identity);
                    db.SaveChanges();
                    appdb.SaveChanges();
                }
                catch {
                    ViewBag.Message = String.Format("Cannot delete this user.");
                    return View(user);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(string id) {
            IdentityUser user = appdb.Users.Where(s => s.Id == id).First();
            UserModel userInfo = db.Users.Where(s => s.ID == id).First();

            CompleteUserModel cuser = new CompleteUserModel { user = user, userInfo = userInfo };

            List<UserRoleViewModel> cat1 = new List<UserRoleViewModel>();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appdb));

            var usr = userManager.FindById(id);
            var roles = appdb.Roles;  
            var userRoles = usr.Roles;

            foreach (IdentityUserRole role in userRoles.ToList()) {
                var roleName = roles.First(s => s.Id == role.RoleId);
                cat1.Add(new UserRoleViewModel { RoleId = role.RoleId, RoleName = roleName.Name });
            }

            ViewBag.roles = cat1;

            return View(cuser);
        }
    }
}