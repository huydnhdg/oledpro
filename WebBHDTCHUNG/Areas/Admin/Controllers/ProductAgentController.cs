using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHDT_OledPro.Areas.Admin.Data;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize]
    public class ProductAgentController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        public ActionResult Index()
        {
            if (User.Identity.Name == "administrator")
            {
                var model1 = from a in db.ProductAgents
                             join b in db.Products on a.ProductId equals b.Id
                             join c in db.AspNetUsers on a.AgentId equals c.Id
                             select new ProductAgentViewModel()
                             {
                                 Id = a.Id,
                                 ProductName = b.Name,
                                 Serial = b.Serial,
                                 AgentName = c.UserName,
                                 Createdate = a.Createdate,
                                 Createby = a.Createby,
                                 Type = a.Type,
                                 Importdate = a.Importdate,
                             };
                return View(model1);
            }
            string userId = User.Identity.GetUserId();
            var model = from a in db.ProductAgents
                        join b in db.Products on a.ProductId equals b.Id
                        where b.Createby == Utility.IdPatner //Vincent
                        join c in db.AspNetUsers on a.AgentId equals c.Id
                        select new ProductAgentViewModel()
                        {
                            Id = a.Id,
                            ProductName = b.Name,
                            Serial = b.Serial,
                            AgentName = c.UserName,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Type = a.Type,
                            Importdate = a.Importdate,
                        };
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] ProductWarranti productWarranti)
        {
            if (ModelState.IsValid)
            {
                //Vincent
                productWarranti.Createby = Utility.IdPatner;
                productWarranti.Createdate = DateTime.Now;
                db.ProductWarrantis.Add(productWarranti);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productWarranti);

        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductAgent productAgent = db.ProductAgents.Find(id);
            if (productAgent == null)
            {
                return HttpNotFound();
            }
            return View(productAgent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] ProductAgent productAgent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productAgent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productAgent);
        }
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductAgent productAgent = db.ProductAgents.Find(id);
            if (productAgent == null)
            {
                return HttpNotFound();
            }
            return View(productAgent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProductAgent productAgent = db.ProductAgents.Find(id);
            db.ProductAgents.Remove(productAgent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}