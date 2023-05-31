using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BHDT_OledPro.Utils;

namespace BHDT_OledPro.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            logger.Info(Utility.getclientIP);
            logger.Info(userId, User.Identity.Name);
            return View();
        }
    }
}