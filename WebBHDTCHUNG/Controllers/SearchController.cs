using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;
using Microsoft.Ajax.Utilities;
using BHDT_OledPro.Data;

namespace BHDT_OledPro.Controllers
{
    [RoutePrefix("tra-cuu")]
    public class SearchController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();

        [Route]
        public ActionResult Index()
        {
            return View();
        }           

        [HttpPost]
        public ActionResult GetProduct(string phoneString)
        {
            phoneString = Utils.FormatString.formatUserId(phoneString, 0);
            logger.Info(string.Format("{0}", phoneString));
            //phoneString = phoneString.TrimStart('0');
            var customer = db.Customers.Where(a => a.Createby == Utility.IdPatner).Where(b => b.Phone == phoneString).FirstOrDefault();
            //var product = db.Products.Where(a=>a.Createby == Utility.IdPatner).Where(a => a.Serial == serial).SingleOrDefault();
            if (customer == null)
            {
                logger.Info("phone not found or valid");
                return Json(ResResult("Số điện thoại không tồn tại!", null), JsonRequestBehavior.AllowGet);//khách hàng khong ton tai
            }
            else
            {                
                var model = (from c in db.ProductActives
                             join r in db.Products on c.ProductId equals r.Id
                             join cc in db.Customers on c.CustomerId equals cc.Id
                             where c.CustomerId == customer.Id
                             //where r.Status == 1
                             select new ProductActiveModel
                             {
                                 CustomerName = customer.Name,
                                 CustomerPhone = customer.Phone,
                                 CustomerAddress = customer.Address,
                                 CustomerProvince = customer.City,
                                 Name = r.Name,
                                 Serial = r.Serial,
                                 Model = r.Model,
                                 ProductCode = c.ProductCode,
                                 Car_Brandname = c.CarBrandname,
                                 AgentAddress = c.InstallationAgentAddress,
                                 MadeIn = r.MadeIn,
                                 Limited = r.Limited,
                                 Activedate = c.Activedate                                 
                             }).ToList();

                logger.Info("phone ok");
                return Json(ResResult("Thông tin các sản phẩm tìm được", model), JsonRequestBehavior.AllowGet);//san pham ok 
            }

        }

        public string ResResult(string message, List<ProductActiveModel> model)
        {
            Ress ress = new Ress()
            {
                message = message,
                prodActive = model
            };
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(ress);//convert to json
            return result;
        }
        public class Ress
        {
            public string message { get; set; }
            public List<ProductActiveModel> prodActive { get; set; }
        }
    }
}