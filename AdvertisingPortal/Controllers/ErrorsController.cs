using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingPortal.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult AccessDanied()
        {
            return View();
        }
    }
}