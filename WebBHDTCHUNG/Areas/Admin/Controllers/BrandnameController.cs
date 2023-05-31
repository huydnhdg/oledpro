using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize]
    public class BrandnameController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        private string userId = "";
        public ActionResult Index()
        {
            if (User.Identity.Name == "administrator")
            {
                var model1 = db.Brandnames.OrderByDescending(m => m.Createdate);
                return View(model1);
            }
            //Vincent
            userId = User.Identity.GetUserId();
            var model = db.Brandnames.Where(a => a.Createby == Utility.IdPatner).OrderByDescending(m => m.Createdate);            
            return View(model);
        }

        public ActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Send(string lsphone, string message)
        {
            if (ModelState.IsValid)
            {
                //xóa dấu cách trong ds sđt
                lsphone = Regex.Replace(lsphone, @"\s", "");
                //kiểm tra brandname của tài khoản
                var brand = db.TempBrandnames.Find(userId);
                string[] words = lsphone.Split(';');
                if (words.Length > 0)
                {
                    foreach( var item in words)
                    {
                        //send brandname to phone
                        int send = Utils.SendMTBrandname.SentMTMessage(message, item, brand.ShowName, brand.ShowName, "0");
                        Brandname brandName = new Brandname()
                        {
                            Status = send,
                            Message = message,
                            Createdate = DateTime.Now,
                            PhoneSend = item
                        };
                        db.Brandnames.Add(brandName);
                        db.SaveChanges();
                    }
                }
                
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}