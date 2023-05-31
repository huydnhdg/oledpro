using Microsoft.Ajax.Utilities;
using NLog;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;

namespace BHDT_OledPro.ReceviceMo
{
    public class ApiReceviceController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        [HttpGet]
        public HttpResponseMessage MO(string Command_Code, string User_ID, string Service_ID, string Request_ID, string Message)
        {
            logger.Info(string.Format("{0} {1} {2} {3} {4}", Command_Code, User_ID, Service_ID, Request_ID, Message));
            //http://ewarranty.formathome.com.vn/api/apirecevice/mo?Command_Code=&User_ID=&Service_ID=&Request_ID=&Message=
            //http://kichhoat.baohanhdientu.vip/api/apirecevice/mo?command_code=&user_id=&service_id=&request_id=&message=
            //https://localhost:44386/api/apirecevice/mo?command_code=BHDT&user_id=0965433459&service_id=1&request_id=1&message=BHDT%20TEKA%20TC%20115
            //kiểm tra command code đã khai báo chưa
            //TEKA BH SERIAL mã to
            //BHDT TEKA BH SERIAL mã con

            if (Command_Code == "BHDT")
            {
                string[] words = Message.ToUpper().Split(' ');
                string cuphap = "BHDT " + words[1];
                var user = db.TempSms.Where(a => a.Command_Code == cuphap).SingleOrDefault();
                if (user != null)
                {
                    return smsMaCon(words, User_ID, user.Id);
                }
                else//cu phap khong dung
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StringContent("-4|" + "Tin nhan khong dung cu phap");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    logger.Info("Tin nhan khong dung cu phap");
                    return response;
                }
            }
            else
            {
                var user = db.TempSms.Where(a => a.Command_Code == Command_Code).SingleOrDefault();
                if (user != null)
                {
                    return smsMaTo(Message, User_ID, user.Id);
                }
                else
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StringContent("-4|" + "Khong dung Command_Code");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    logger.Info("Khong dung Command_Code");
                    return response;
                }
            }
        }
        public HttpResponseMessage smsMaCon(string[] words, string User_ID, string partner)
        {
            string MT = "";

            if (words[2] == "TC")
            {
                MT = getProduct(User_ID, words[3], partner);
            }
            else if (words[2] == "BH")
            {
                MT = warrantiProduct(User_ID, partner);
            }
            else
            {
                if (words.Length == 4)
                {
                    //check xem co dung la so dien thoai khong
                    words[3] = Utils.FormatString.formatUserId(words[3], 0);
                    MT = activeProduct(User_ID, words[2], words[3], partner);
                }
                else
                {
                    MT = activeProduct(User_ID, words[2], null, partner);
                }
            }

            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|" + MT);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            logger.Info(MT);
            return response;
        }
        public HttpResponseMessage smsMaTo(string Message, string User_ID, string partner)
        {
            string MT = "";
            string[] words = Message.ToUpper().Split(' ');

            if (words[1] == "TC")
            {
                MT = getProduct(User_ID, words[2], partner);
            }
            else if (words[1] == "BH")
            {
                MT = warrantiProduct(User_ID, partner);
            }
            else
            {
                if (words.Length == 3)
                {
                    //check xem co dung la so dien thoai khong
                    words[2] = Utils.FormatString.formatUserId(words[2], 0);
                    MT = activeProduct(User_ID, words[1], words[2], partner);
                }
                else
                {
                    MT = activeProduct(User_ID, words[1], null, partner);
                }
            }

            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|" + MT);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            logger.Info(MT);
            return response;
        }
        public string getProduct(string phone, string serial, string partner)
        {
            var product = db.Products.Where(i => i.Createby == partner && i.Serial == serial).SingleOrDefault();

            if (product != null)
            {
                var partnerCode = db.TempSms.Find(product.Createby);
                if (product.Status == null)
                {
                    //san pham chua kich hoat
                    return SmsTemp.TRACUU_NOACTIVE(FormatString.convertToUnSign3(product.Name), serial, partnerCode.Command_Code);
                }
                else
                {
                    //san pham da kich hoat
                    var prodactive = db.ProductActives.Where(i => i.ProductId == product.Id).SingleOrDefault();
                    string adate = prodactive.Activedate.Value.ToString("dd/MM/yyyy");
                    string edate = prodactive.Activedate.Value.AddMonths(product.Limited ?? default(int)).ToString("dd/MM/yyyy");
                    return SmsTemp.TRACUU_ACTIVE(FormatString.convertToUnSign3(product.Name), serial, adate, edate);
                }
            }
            else
            {
                return SmsTemp.TRACUU_NOTVALID();
            }
        }
        public string activeProduct(string phonekt, string serial, string phone, string partner)
        {
            var product = db.Products.Where(i => i.Createby == partner && i.Serial == serial).SingleOrDefault();
            if (product != null && product.Status == null)//du dieu kien kich hoat
            {
                var partnerCode = db.TempSms.Find(product.Createby);
                

                if (phone != null)
                {
                    phone = Utils.FormatString.formatUserId(phone, 0);
                }
                else
                {
                    if (phonekt != null)
                    {
                        phonekt = Utils.FormatString.formatUserId(phonekt, 0);
                        phone = phonekt;
                    }
                    else
                    {
                        phone = null;
                    }
                }

                //add thong tin khach hang
                var cus = db.Customers.Where(a => a.Phone == phone).FirstOrDefault();
                long idcus;
                if (cus == null)
                {
                    var newcus = new Customer()
                    {
                        Phone = phone,
                        Createdate = DateTime.Now,
                        Createby = product.Createby
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
                //add thong tin kich hoat
                if (!String.IsNullOrEmpty(phonekt))
                {
                    var prodac = new ProductActive()
                    {
                        Activedate = DateTime.Now,
                        ProductId = product.Id,
                        CustomerId = idcus,
                        Type = 3,
                        Activeby = phonekt
                    };
                    db.ProductActives.Add(prodac);
                }
                else
                {
                    var prodac = new ProductActive()
                    {
                        Activedate = DateTime.Now,
                        ProductId = product.Id,
                        CustomerId = idcus,
                        Type = 3
                    };
                    db.ProductActives.Add(prodac);
                }


                //update status table product
                product.Status = 1;
                db.Entry(product).State = EntityState.Modified;
                //luu vao db
                db.SaveChanges();
                //tra lai MT cho khach hang
                string adate = DateTime.Now.ToString("dd/MM/yyyy");
                string edate = DateTime.Now.AddMonths(product.Limited ?? default(int)).ToString("dd/MM/yyyy");
                return SmsTemp.ACTIVE(FormatString.convertToUnSign3(product.Name), product.Serial, adate, edate);
            }
            else if (product != null && product.Status == 1)
            {
                //san pham da kich hoat
                var prodactive = db.ProductActives.Where(i => i.ProductId == product.Id).SingleOrDefault();
                string adate = prodactive.Activedate.Value.ToString("dd/MM/yyyy");
                string edate = prodactive.Activedate.Value.AddMonths(product.Limited ?? default(int)).ToString("dd/MM/yyyy");
                return SmsTemp.TRACUU_ACTIVE(FormatString.convertToUnSign3(product.Name), serial, adate, edate);
            }
            else//khong du dieu kien kich hoat
            {
                return SmsTemp.ACTIVE_NOTVALID();
            }

        }

        public string warrantiProduct(string phone, string partner)
        {

            var user = db.Customers.Where(a => a.Phone == phone).FirstOrDefault();
            if (user != null)
            {
                //add thong tin bao hanh co thong tin khach hang
                var warranti = new ProductWarranti()
                {
                    CustomerId = user.Id,//id khach hang
                    PhoneWarranti = phone,
                    Createdate = DateTime.Now,
                    Createby = partner
                };
                db.ProductWarrantis.Add(warranti);
            }
            else
            {
                //add thogn tin bao hanh khong co thong tin khach hang
                Customer customer = new Customer()
                {
                    Phone = phone,
                    Createdate = DateTime.Now,
                    Createby= partner
                };
                db.Customers.Add(customer);
                db.SaveChanges();
                var warranti = new ProductWarranti()
                {
                    CustomerId = customer.Id,
                    PhoneWarranti = phone,
                    Createdate = DateTime.Now,
                    Createby = partner
                };
                db.ProductWarrantis.Add(warranti);
            }
            db.SaveChanges();
            return SmsTemp.WARRANTI();
        }
    }
}
