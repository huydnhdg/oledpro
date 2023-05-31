using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;

namespace BHDT_OledPro.Controllers
{
    [RoutePrefix("thong-tin-dai-ly")]
    [Authorize(Roles = "Agent")]
    public class AgentInforController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        [Route]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var model = from a in db.ProductAgents
                        where a.AgentId == userId
                        join b in db.Products on a.ProductId equals b.Id
                        where b.Createby == Utility.IdPatner
                        join c in db.ProductActives on a.ProductId equals c.ProductId
                        into temp
                        from t in temp.DefaultIfEmpty()
                        select new StoreViewModel()
                        {
                            NgayKH = t.Activedate,
                            NgayNK = a.Createdate,
                            Name = b.Name,
                            Serial=b.Serial,
                            Code=b.Code,
                            Limited=b.Limited,

                        };

            ViewBag.user = db.AspNetUsers.Find(userId);
            ViewBag.modelactive = model.Where(a => a.NgayKH != null).Count();
            ViewBag.modelnoactive = model.Where(a => a.NgayKH == null).Count();
            return View(model);
        }
    }
}