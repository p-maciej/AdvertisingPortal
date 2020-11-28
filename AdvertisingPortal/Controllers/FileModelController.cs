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
    public class FileModelController : Controller {
        private AdvertisementPortalContext db = new AdvertisementPortalContext();
        public ActionResult Index() {
            DbSet<FileModel> files = db.Files;
            return View(files.ToList());
        }
    }
}