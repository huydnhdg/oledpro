using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebBHDTCHUNG.Utils
{
    public class UpdateSMSCustomer
    {
        static private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();

        public static void UpdateSMSCus()
        {
            var productActive = db.ProductActives.Where(z => z.CustomerId == 133);
            var li = productActive.ToList();
            var cou = productActive.Count();

            foreach(var item in li)
            {
                var phone = item.Activeby;
                var cus = db.Customers.Where(a => a.Phone == phone).FirstOrDefault();
                long idcus;
                if (cus == null)
                {
                    var newcus = new Customer()
                    {
                        Phone = phone,
                        Createdate = DateTime.Now,
                        Createby = Utility.IdPatner
                    };
                    db.Customers.Add(newcus);
                    db.SaveChanges();
                    db.Entry(newcus).GetDatabaseValues();
                    idcus = newcus.Id;//gan id khach hang
                }
                else
                {
                    idcus = cus.Id;
                }

                var productActiveForUpdate = db.ProductActives.Where(x => x.Id == item.Id).FirstOrDefault();
                productActiveForUpdate.CustomerId = idcus;
                db.Entry(productActiveForUpdate).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}