﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvertisingPortal.DAL;
using AdvertisingPortal.Models;

namespace AdvertisingPortal.Controllers {
    public class UserModelController : Controller {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();

        public ActionResult Index() {
            DbSet<UserModel> users = db.Users;
            return View(users.ToList());
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "FirstName, LastName, PhoneNumber, Email, City")] UserModel userModel) { 
            if(ModelState.IsValid) {
                db.Users.Add(userModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userModel);
        }
    }
}