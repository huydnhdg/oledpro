using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using BHDT_OledPro.Areas.Admin.Data;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        private string userId = "";
        public ActionResult Index()
        {
            //userId = User.Identity.GetUserId();
            //Vincent
            userId = Utility.IdPatner;
            if (User.Identity.Name == "administrator")
            {
                var model = from a in db.ProductActives
                            join b in db.Products on a.ProductId equals b.Id
                            select a;
                var war = from a in db.ProductWarrantis
                          join b in db.Products on a.ProductId equals b.Id
                          select a;
                ViewBag.product = db.Products.Count();
                ViewBag.active = model.Count();
                ViewBag.warranti = war.Count();
                ViewBag.agent = db.AspNetUsers.Count();
            }
            else
            {
                var model = from a in db.ProductActives
                            join b in db.Products on a.ProductId equals b.Id
                            where b.Createby == userId
                            select a;

                var war = from a in db.ProductWarrantis
                          join b in db.Products on a.ProductId equals b.Id
                          where b.Createby == userId
                          select a;

                List<DataPoint> dataPoints = new List<DataPoint>();

                double webcus = model.Where(i => i.Type == 1).Count();
                double webage = model.Where(i => i.Type == 2).Count();
                double smscus = model.Where(i => i.Type == 3).Count();
                double smsage = model.Where(i => i.Type == 4).Count();
                double suma = model.Count();
                double wc = webcus / suma * (double)100;
                double wa = webage / suma * (double)100;
                double sc = smscus / suma * (double)100;
                double sa = smsage / suma * (double)100;
                dataPoints.Add(new DataPoint("Khách hàng KH Web", wc));
                dataPoints.Add(new DataPoint("Đại lý KH Web", wa));
                dataPoints.Add(new DataPoint("Khách hàng KH Sms", sc));
                dataPoints.Add(new DataPoint("Đại lý KH Sms", sa));
                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

                List<DataPoint> dataPoints1 = new List<DataPoint>();

                int t1 = model.Where(a => (a.Activedate.Value.Month) == 1).Count();
                int t2 = model.Where(a => (a.Activedate.Value.Month) == 2).Count();
                int t3 = model.Where(a => (a.Activedate.Value.Month) == 3).Count();
                int t4 = model.Where(a => (a.Activedate.Value.Month) == 4).Count();
                int t5 = model.Where(a => (a.Activedate.Value.Month) == 5).Count();
                int t6 = model.Where(a => (a.Activedate.Value.Month) == 6).Count();
                int t7 = model.Where(a => (a.Activedate.Value.Month) == 7).Count();
                int t8 = model.Where(a => (a.Activedate.Value.Month) == 8).Count();
                int t9 = model.Where(a => (a.Activedate.Value.Month) == 9).Count();
                int t10 = model.Where(a => (a.Activedate.Value.Month) == 10).Count();
                int t11 = model.Where(a => (a.Activedate.Value.Month) == 11).Count();
                int t12 = model.Where(a => (a.Activedate.Value.Month) == 12).Count();
                dataPoints1.Add(new DataPoint("Thang 1", t1));
                dataPoints1.Add(new DataPoint("Thang 2", t2));
                dataPoints1.Add(new DataPoint("Thang 3", t3));
                dataPoints1.Add(new DataPoint("Thang 4", t4));
                dataPoints1.Add(new DataPoint("Thang 5", t5));
                dataPoints1.Add(new DataPoint("Thang 6", t6));
                dataPoints1.Add(new DataPoint("Thang 7", t7));
                dataPoints1.Add(new DataPoint("Thang 8", t8));
                dataPoints1.Add(new DataPoint("Thang 9", t9));
                dataPoints1.Add(new DataPoint("Thang 10", t10));
                dataPoints1.Add(new DataPoint("Thang 11", t11));
                dataPoints1.Add(new DataPoint("Thang 12", t12));
                ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1);

                ViewBag.product = db.Products.Where(a => a.Createby == userId).Count();
                ViewBag.active = model.Count();
                ViewBag.warranti = war.Count();
                ViewBag.agent = db.AspNetUsers.Where(a => a.Createby == userId).Count();


                List<DataPoint> dataPoints2 = new List<DataPoint>();
                var activeHN = from a in db.ProductActives
                               join b in db.Customers on a.CustomerId equals b.Id
                               where b.City.Contains("Hà Nội")
                               select a;
                var activeDN = from a in db.ProductActives
                               join b in db.Customers on a.CustomerId equals b.Id
                               where b.City.Contains("Đà Nẵng")
                               select a;
                var activeHCM = from a in db.ProductActives
                                join b in db.Customers on a.CustomerId equals b.Id
                                where b.City.Contains("Hồ Chí Minh")
                                select a;
                double at = suma - (double)activeHN.Count() - (double)activeDN.Count() - (double)activeHCM.Count();
                double ah = (double)activeHN.Count() / suma * (double)100;
                double ad = (double)activeDN.Count() / suma * (double)100;
                double am = (double)activeHCM.Count() / suma * (double)100;
                double aout = at / suma * (double)100;
                dataPoints2.Add(new DataPoint("Hà Nội", ah));
                dataPoints2.Add(new DataPoint("Đà Nẵng", ad));
                dataPoints2.Add(new DataPoint("Hồ Chí Minh", am));
                dataPoints2.Add(new DataPoint("Khác", aout));
                ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2);

                var pie = db.PieNames.Find(userId);
                List<DataPoint> dataPoints3 = new List<DataPoint>();
                var sp1 = from a in db.ProductActives
                          join b in db.Products on a.ProductId equals b.Id
                          where b.Name.Contains(pie.Name1)
                          select a;
                var sp2 = from a in db.ProductActives
                          join b in db.Products on a.ProductId equals b.Id
                          where b.Name.Contains(pie.Name2)
                          select a;
                var sp3 = from a in db.ProductActives
                          join b in db.Products on a.ProductId equals b.Id
                          where b.Name.Contains(pie.Name3)
                          select a;
                var sp4 = from a in db.ProductActives
                          join b in db.Products on a.ProductId equals b.Id
                          where b.Name.Contains(pie.Name4)
                          select a;
                double ak = suma - (double)sp1.Count() - (double)sp2.Count() - (double)sp3.Count() - (double)sp4.Count();
                double a1 = (double)sp1.Count() / suma * (double)100;
                double a2 = (double)sp2.Count() / suma * (double)100;
                double a3 = (double)sp3.Count() / suma * (double)100;
                double a4 = (double)sp4.Count() / suma * (double)100;
                double a5 = ak / suma * (double)100;
                dataPoints3.Add(new DataPoint(pie.Name1, a1));
                dataPoints3.Add(new DataPoint(pie.Name2, a2));
                dataPoints3.Add(new DataPoint(pie.Name3, a3));
                dataPoints3.Add(new DataPoint(pie.Name4, a4));
                dataPoints3.Add(new DataPoint(pie.Name5, a5));
                ViewBag.DataPoints3 = JsonConvert.SerializeObject(dataPoints3);
            }



            return View();
        }

        public PartialViewResult ShowMenu()
        {
            userId = User.Identity.GetUserId();
            var user = db.TempBrandnames.Find(userId);
            if (user != null)
            {
                ViewBag.brand = user.ShowName;
            }

            return PartialView();
        }
    }
}