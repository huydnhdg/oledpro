using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    public class PieNameController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        private string userId = "";
        public ActionResult Index()
        {
            //Vincent
            //userId = User.Identity.GetUserId();
            userId = Utility.IdPatner;
            var model = db.PieNames.Where(a => a.Id == userId);
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PieName pie = db.PieNames.Where(a => a.Id == id).SingleOrDefault();
            if (pie == null)
            {
                return HttpNotFound();
            }
            return View(pie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] PieName pie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pie);
        }
    }
}