using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;
using PagedList;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        public ActionResult Index(int? page, string SearchString, string currentFilter)
        {

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;

            }
            if (User.Identity.Name == "administrator")
            {
                var model1 = new List<Customer>();
                if (!String.IsNullOrEmpty(SearchString))
                    model1 = db.Customers.Where(a => a.Phone.Contains(SearchString)).ToList();
                else
                {
                    model1 = db.Customers.OrderByDescending(m => m.Createdate).ToList();
                }

                ViewBag.currentFilter = SearchString;
                return View(model1.ToPagedList(pageNumber, pageSize));

            }
            //Vincent
            string userId = User.Identity.GetUserId();
            var model = new List<Customer>();

            if (!String.IsNullOrEmpty(SearchString))
                model = db.Customers.Where(a => a.Phone.Contains(SearchString)).ToList();
            else
            {
                model = db.Customers.Where(a => a.Createby == Utility.IdPatner).OrderByDescending(m => m.Createdate).ToList();
                var c = model.Count();
            }
            ViewBag.currentFilter = SearchString;
            return View(model.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult FillDistricts(string City)
        {
            var id = db.Provinces.Where(s => s.Name == City).SingleOrDefault();
            var provi = db.Districts.Where(x => x.ProvinceId == id.Id).ToList();
            var ress = new List<String>();
            foreach (var i in provi)
            {
                ress.Add(i.Name);
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(ress);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.Phone = FormatString.formatUserId(customer.Phone, 0);
                //Vincent
                customer.Createby = Utility.IdPatner; //change to take Partner ID
                customer.Createdate = DateTime.Now;
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);

        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.City = new SelectList(db.Provinces.ToList(), "Name", "Name", id);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.Phone = FormatString.formatUserId(customer.Phone, 0);
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.City = new SelectList(db.Provinces.ToList(), "Name", "Name", customer.Id);
            return View(customer);
        }
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}