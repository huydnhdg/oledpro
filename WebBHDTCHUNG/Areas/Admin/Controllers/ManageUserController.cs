using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHDT_OledPro.Models;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Partner")]
    public class ManageUserController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            IEnumerable<ApplicationUser> model;
            model = context.Users.Where(i => i.Createby == userId).AsEnumerable();
            return View(model);
        }
        public ActionResult Info()
        {
            string userId = User.Identity.GetUserId();
            if(userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser model = db.AspNetUsers.Find(userId);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        public ActionResult EditSMS(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempSm tempSm = db.TempSms.Find(id);
            if (tempSm == null)
            {
                return HttpNotFound();
            }
            return View(tempSm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSMS([Bind(Include = "")] TempSm tempSm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tempSm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tempSm);
        }
        public ActionResult EditBrandName(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempBrandname tempBrandname = db.TempBrandnames.Find(id);
            if (tempBrandname == null)
            {
                return HttpNotFound();
            }
            return View(tempBrandname);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBrandName([Bind(Include = "")] TempBrandname tempBrandname)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tempBrandname).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tempBrandname);
        }
        public ActionResult Edit(string Id)
        {
            ApplicationUser model = context.Users.Find(Id);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicationUser model)
        {
            try
            {
                context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult EditRole(string Id)
        {
            ApplicationUser model = context.Users.Find(Id);
            if (User.IsInRole("Partner"))
            {
                ViewBag.RoleId = new SelectList(
                                context.Roles.ToList().Where(
                                    item => model.Roles.FirstOrDefault(
                                        r => r.RoleId == item.Id) == null
                                        && item.Id != "Admin" && item.Id != "Partner").ToList(), "Id", "Name");
            }
            else
            {
                ViewBag.RoleId = new SelectList(
                                context.Roles.ToList().Where(
                                    item => model.Roles.FirstOrDefault(
                                        r => r.RoleId == item.Id) == null).ToList(), "Id", "Name");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToRole(string UserId, string[] RoleId)
        {
            ApplicationUser model = context.Users.Find(UserId);
            if (RoleId != null && RoleId.Count() > 0)
            {
                foreach (string item in RoleId)
                {
                    IdentityRole role = context.Roles.Find(RoleId);
                    model.Roles.Add(new IdentityUserRole() { UserId = UserId, RoleId = item });
                }
                context.SaveChanges();
            }
            if (User.IsInRole("Partner"))
            {
                ViewBag.RoleId = new SelectList(
                                context.Roles.ToList().Where(
                                    item => model.Roles.FirstOrDefault(
                                        r => r.RoleId == item.Id) == null
                                        && item.Id != "Admin" && item.Id != "Partner").ToList(), "Id", "Name");
            }
            else
            {
                ViewBag.RoleId = new SelectList(
                                context.Roles.ToList().Where(
                                    item => model.Roles.FirstOrDefault(
                                        r => r.RoleId == item.Id) == null).ToList(), "Id", "Name");
            }
            return RedirectToAction("EditRole", new { Id = UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleFromUser(string UserId, string RoleId)
        {
            ApplicationUser model = context.Users.Find(UserId);
            model.Roles.Remove(model.Roles.Single(m => m.RoleId == RoleId));
            context.SaveChanges();
            ViewBag.RoleId = new SelectList(context.Roles.ToList().Where(item => model.Roles.FirstOrDefault(r => r.RoleId == item.Id) == null).ToList(), "Id", "Name");
            return RedirectToAction("EditRole", new { Id = UserId });
        }

        public ActionResult Delete(string Id)
        {
            var model = context.Users.Find(Id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string Id)
        {
            ApplicationUser model = null;
            try
            {
                model = context.Users.Find(Id);
                context.Users.Remove(model);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Delete", model);
            }
        }
    }
}