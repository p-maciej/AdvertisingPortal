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
        private static AdvertisementPortalContext db = new AdvertisementPortalContext();
        private static ApplicationDbContext appdb = new ApplicationDbContext();

        [Authorize(Roles = "admin")]
        public ActionResult Index() {
            List<CompleteUserModel> list = new List<CompleteUserModel>();
            var users = appdb.Users;
            foreach (IdentityUser user in users) {
                UserModel userInfo = db.Users.Find(user.Id);

                list.Add(new CompleteUserModel(user, userInfo));
            }
            return View(list);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(string id) {
            IdentityUser user = appdb.Users.Where(s => s.Id == id).First();
            UserModel userInfo = db.Users.Where(s => s.ID == id).First();


            CompleteUserModel cuser = new CompleteUserModel { user = user, userInfo = userInfo };
            
            List<UserRoleViewModel> cat1 = new List<UserRoleViewModel>();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appdb));

            var usr = userManager.FindById(id);
            var roles = appdb.Roles;
            var userRoles = usr.Roles;

            foreach (var role in roles.ToList()) {
                var roleView = new UserRoleViewModel { RoleId = role.Id, RoleName = role.Name, IsSelected = false };
                foreach (var userRole in userRoles) {
                    if(userRole.RoleId == role.Id) {
                        roleView.IsSelected = true;
                        break;
                    }
                } 

                cat1.Add(roleView);   
            }

            ViewBag.rolesT = cat1;

            return View(cuser);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(CompleteUserModel val, string[] roleGroup) {
            if (ModelState.IsValid) {
                IdentityUser userE = appdb.Users.First(s => s.Id == val.user.Id);
                userE.Email = val.user.Email;
                UserModel userInfo = db.Users.First(s => s.ID == val.user.Id);
                userInfo.FirstName = val.userInfo.FirstName;
                userInfo.LastName = val.userInfo.LastName;
                userInfo.City = val.userInfo.City;
                userInfo.PhoneNumber = val.userInfo.PhoneNumber;

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appdb));
                var roles = appdb.Roles;

                // add to role
                if (roleGroup.Length > 0) {
                    foreach (var role in roleGroup) {
                        var roleName = roles.First(s => s.Id == role);
                        if (!userManager.IsInRole(userE.Id, roleName.Name)) {
                            userManager.AddToRole(userE.Id, roleName.Name);
                        }
                    }
                }

                // remove from unchecked roles
                foreach(var role in roles) {
                    if(!roleGroup.Contains(role.Id)) {
                        try {
                            if (userManager.IsInRole(userE.Id, role.Name)) {
                                System.Diagnostics.Debug.WriteLine(role.Name);
                                userE.Roles.Remove(userE.Roles.First(r => r.RoleId == role.Id));
                            }
                        } catch(Exception e) {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                    }
                }


                appdb.SaveChanges();
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(val);
        }

        [Authorize(Roles = "admin")]
        public ActionResult ConfirmDelete(string id) {
            UserModel user = db.Users.Find(id);
            IdentityUser identity = appdb.Users.Find(id);

            if (user == null) {
                return HttpNotFound();
            }
            return View(new CompleteUserModel(identity, user));
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(string id) {
            if (ModelState.IsValid) {
                UserModel user = db.Users.Find(id);
                ApplicationUser identity = appdb.Users.Find(id);
                try {
                    var userAds = db.Advertisements.Where(u => u.User.ID == user.ID).ToList();

                    foreach(var ad in userAds) {
                        string strPhysicalFolder = Server.MapPath("~/UploadedFiles/");

                        string strFileFullPath = strPhysicalFolder + ad.Files.Path;
                        if (System.IO.File.Exists(strFileFullPath)) {
                            System.IO.File.Delete(strFileFullPath);
                        }

                        db.Files.Remove(ad.Files);

                        db.Advertisements.Remove(ad);
                    }

                    db.Users.Remove(user);
                    appdb.Users.Remove(identity);
                    db.SaveChanges();
                    appdb.SaveChanges();
                }
                catch {
                    ViewBag.Message = String.Format("Cannot delete this user.");
                    return View("ConfirmDelete", new CompleteUserModel(identity, user));
                }
            }
            System.Diagnostics.Debug.WriteLine("delete");
            return RedirectToAction("Index", "UserModel");
        }

        [Authorize(Roles = "admin")]  
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

        public ActionResult AddToFavourite(int id) {
            IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();
            UserModel userInfo = db.Users.Where(s => s.ID == user.Id).First();

            int check = db.Favourites.Where(f => f.Advertisement.ID == id && f.User.ID == userInfo.ID).Count();

            if (check == 0) {
                AdvertisementModel advertisement = db.Advertisements.First(s => s.ID == id);
                db.Favourites.Add(new FavouriteModel { User = userInfo, Advertisement = advertisement });
                db.SaveChanges();
            }

            return RedirectToAction("Details/" + id.ToString(), "AdvertisementModel");
        }

        public ActionResult RemoveFromFavourites(int id, int redirect) {
            IdentityUser user = appdb.Users.Where(s => s.Email == User.Identity.Name).First();

            int check = db.Favourites.Where(f => f.Advertisement.ID == id && f.User.ID == user.Id).Count();

            if (check > 0) {
                FavouriteModel favourite = db.Favourites.First(f => f.Advertisement.ID == id && f.User.ID == user.Id);
                db.Favourites.Remove(favourite);
                db.SaveChanges();
            }

            if (redirect == 0) {
                return RedirectToAction("UserDetails", "Home");
            } else {
                return RedirectToAction("Details/" + id.ToString(), "AdvertisementModel");
            }    
        }
    }
}