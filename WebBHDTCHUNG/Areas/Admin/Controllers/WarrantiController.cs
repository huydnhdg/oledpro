using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BHDT_OledPro.Areas.Admin.Data;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;
using PagedList;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize]
    public class WarrantiController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        public ActionResult Index(int? page, int? status)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (User.Identity.Name == "administrator")
            {
                var model1 = from a in db.ProductWarrantis
                             join b in db.Products on a.ProductId equals b.Id
                             into temp1
                             from t1 in temp1.DefaultIfEmpty()
                             join c in db.Customers on a.CustomerId equals c.Id
                             into temp2
                             from t2 in temp2.DefaultIfEmpty()
                             orderby a.Createdate
                             select new WarrantiViewModel()
                             {
                                 ProductName = t1.Name,
                                 Serial = t1.Serial,
                                 CustomerName = t2.Name,
                                 CustomerAddress = t2.Address,
                                 CustomerProvince = t2.City,
                                 InstallationAgentAddress = a.InstallationAgentAddress,
                                 CarBrandname = a.CarBrandname,
                                 ProductCode = a.ProductCode,
                                 PhoneWarranti = a.PhoneWarranti,
                                 Category = a.Category,
                                 Status = a.Status,
                                 Note = a.Note,
                                 Createdate = a.Createdate,
                                 Createby = a.Createby,
                                 Checkdate = a.Checkdate,
                                 Checkby = a.Checkby,
                                 Id = a.Id,
                             };

                if (status != null)
                {
                    model1 = model1.Where(c => c.Status == status);
                    ViewBag.Channel = status;
                }
                return View(model1.ToPagedList(pageNumber, pageSize));
            }

            //Vincent
            string userId = User.Identity.GetUserId();
            var model = from a in db.ProductWarrantis
                        where a.Createby == Utility.IdPatner // Vincent
                        join b in db.Products on a.ProductId equals b.Id
                        into temp1
                        from t1 in temp1.DefaultIfEmpty()
                        join c in db.Customers on a.CustomerId equals c.Id
                        into temp2
                        from t2 in temp2.DefaultIfEmpty()
                        orderby a.Createdate
                        select new WarrantiViewModel()
                        {
                            ProductName = t1.Name,
                            Serial = t1.Serial,
                            CustomerName = t2.Name,
                            CustomerAddress = t2.Address,
                            CustomerProvince = t2.City,
                            InstallationAgentAddress = a.InstallationAgentAddress,
                            CarBrandname = a.CarBrandname,
                            ProductCode = a.ProductCode,
                            PhoneWarranti = a.PhoneWarranti,
                            Category = a.Category,
                            Status = a.Status,
                            Note = a.Note,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Checkdate = a.Checkdate,
                            Checkby = a.Checkby,
                            Id = a.Id,
                        };


            if (status != null)
            {
                model = model.Where(c => c.Status == status);
                ViewBag.Channel = status;
            }
            return View(model.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            //Vincent
            //string userId = User.Identity.GetUserId();
            string userId = Utility.IdPatner;
            //ViewBag.ProductId = new SelectList(db.Products.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            TempData["ProductId"] = new SelectList(db.Products.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            ViewBag.CustomerId = new SelectList(db.Customers.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] ProductWarranti productWarranti)
        {
            if (!ModelState.IsValid)
            {
                //Vincent
                //productWarranti.Createby = User.Identity.GetUserId();
                productWarranti.Createby = Utility.IdPatner;
                productWarranti.Createdate = DateTime.Now;

                db.ProductWarrantis.Add(productWarranti);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            string userId = Utility.IdPatner;
            //ViewBag.ProductId = new SelectList(db.Products.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            TempData["ProductId"] = new SelectList(db.Products.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            ViewBag.CustomerId = new SelectList(db.Customers.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            return View(productWarranti);
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductWarranti w = db.ProductWarrantis.Find(id);

            WarrantiViewModel productWarranti = new WarrantiViewModel()
            {
                Id = w.Id,
                ProductId = w.ProductId,
                CustomerId = w.CustomerId,
                CarBrandname = w.CarBrandname,
                InstallationAgentAddress = w.InstallationAgentAddress,
                ProductCode = w.ProductCode,
                PhoneWarranti = w.PhoneWarranti,
                Category = w.Category,
                Status = w.Status,
                Note = w.Note,
                Createdate = w.Createdate,
                Createby = w.Createby,
                Checkdate = w.Checkdate,
                Checkby = w.Checkby
            };
            if (productWarranti == null)
            {
                return HttpNotFound();
            }
            return View(productWarranti);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] WarrantiViewModel productWarranti)
        {
            if (ModelState.IsValid)
            {
                ProductWarranti pw = db.ProductWarrantis.Find(productWarranti.Id);
                pw.Checkby = User.Identity.GetUserId();
                pw.Checkdate = DateTime.Now;
                pw.Status = productWarranti.Status;
                pw.Note = productWarranti.Note;
                pw.Category = productWarranti.Category;
                pw.PhoneWarranti = productWarranti.PhoneWarranti;
                var getid = db.Products.Where(a => a.Createby == productWarranti.Createby).Where(a => a.Serial == productWarranti.Serial).SingleOrDefault();
                if (getid != null)
                {
                    pw.ProductId = getid.Id;
                }
                db.Entry(pw).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productWarranti);
        }
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductWarranti productWarranti = db.ProductWarrantis.Find(id);
            if (productWarranti == null)
            {
                return HttpNotFound();
            }
            return View(productWarranti);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProductWarranti productWarranti = db.ProductWarrantis.Find(id);
            db.ProductWarrantis.Remove(productWarranti);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}