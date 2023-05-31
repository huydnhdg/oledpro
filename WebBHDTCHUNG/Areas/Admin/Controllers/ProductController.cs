

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Data.Entity;
using BHDT_OledPro.Areas.Admin.Data;
using BHDT_OledPro.Models;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using System.Web.Script.Serialization;
using BHDT_OledPro.Utils;
using NLog;
using PagedList;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        private string userId = "";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static List<Product> listExc = new List<Product>();
        //public ActionResult Index()

        //{
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string startDate, string endDate, string currentStart, string currentEnd, string sdate, string edate, string currentS, string currentE, string status, string currentStatus)
        {
            //if (User.Identity.Name == "administrator")
            //{
            //    var model1 = db.Products.OrderByDescending(m => m.Createdate);
            //    return View(model1);
            //}
            ////Vincent
            ///
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            if (startDate != null)
            {
                page = 1;
            }
            else
            {
                startDate = currentStart;
            }
            ViewBag.currentStart = startDate;
            if (endDate != null)
            {
                page = 1;
            }
            else
            {
                endDate = currentEnd;
            }
            ViewBag.currentEnd = endDate;
            if (sdate != null)
            {
                page = 1;
            }
            else
            {
                sdate = currentS;
            }
            ViewBag.currentS = sdate;
            if (edate != null)
            {
                page = 1;
            }
            else
            {
                edate = currentE;
            }
            ViewBag.currentE = edate;
            if (status != null)
            {
                page = 1;
            }
            else
            {
                status = currentStatus;
            }
            ViewBag.currentStatus = status;
            // userId = User.Identity.GetUserId();
            var model = db.Products.Where(a => a.Createby == Utility.IdPatner);//.OrderByDescending(a => a.Createdate);

            // Tìm theo tên sản phẩm, model, serial ...
            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(s => s.Name.Contains(searchString)
                                       || s.Serial.Contains(searchString)
                                       || s.Model.Contains(searchString)
                                       || s.Code.Contains(searchString)
                                       );
            }

            if (!String.IsNullOrEmpty(startDate))
            {
                DateTime d = DateTime.Parse(startDate);
                model = model.OrderByDescending(a => a.Exportdate).Where(s => s.Exportdate >= d);
            }
            if (!String.IsNullOrEmpty(endDate))
            {
                DateTime d = DateTime.Parse(endDate);
                d = d.AddDays(1);
                model = model.OrderByDescending(a => a.Exportdate).Where(s => s.Exportdate <= d);
            }

            if (!String.IsNullOrEmpty(sdate))
            {
                DateTime d = DateTime.Parse(sdate);
                model = model.OrderByDescending(a => a.Importdate).Where(s => s.Importdate >= d);
            }
            if (!String.IsNullOrEmpty(edate))
            {
                DateTime d = DateTime.Parse(edate);
                d = d.AddDays(1);
                model = model.OrderByDescending(a => a.Importdate).Where(s => s.Importdate <= d);
            }
            model = model.OrderByDescending(a => a.Createdate);

            listExc = model.ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(listExc.ToPagedList(pageNumber, pageSize));
            // return View(model);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Createby = Utility.IdPatner; //Vincent
                product.Createdate = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);

        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult Active(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ActiveViewModel active = new ActiveViewModel()
            {
                ProductId = product.Id
            };
            TempData["province"] = getProvince();
            return View(active);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Active([Bind(Include = "")] ActiveViewModel active)
        {
            if (ModelState.IsValid)
            {
                //update product
                var prod = db.Products.Find(active.ProductId);
                //add customer
                long cusId = addCustomer(active.CustomerPhone, active.CustomerName, active.City, active.District, active.Address, active.Email, userId);
                //add productactive
                addActive(prod, cusId);
                //gui brandname neu partner dang ki
                checkSendBrandname(prod.Createby, active.CustomerPhone, prod.Serial, active.Activedate.Value.ToString("dd/MM/yyyy"), active.Activedate.Value.AddMonths(prod.Limited ?? default(int)).ToString("dd/MM/yyyy"));
                return RedirectToAction("Index");
            }
            return View(active);
        }

        private long addCustomer(string phone, string name, string province, string district, string address, string email, string create)
        {
            var customer = db.Customers.Where(a => a.Phone == phone).FirstOrDefault();
            if (customer != null)
            {
                //Vincent
                if (String.IsNullOrEmpty(customer.Name))
                {
                    customer.Name = name;
                    customer.City = province;
                    customer.District = district;
                    customer.Address = address;
                    customer.Email = email;
                    db.Entry(customer).State = EntityState.Modified;
                    return db.SaveChanges();
                }
                else
                {
                    return customer.Id;
                }

            }
            else
            {
                var newcus = new Customer()
                {
                    Phone = phone,
                    Name = name,
                    City = province,
                    District = district,
                    Address = address,
                    Email = email,
                    Createdate = DateTime.Now,
                    Createby = create
                };
                db.Customers.Add(newcus);
                db.SaveChanges();
                return newcus.Id;
            }
        }
        private void addActive(Product product, long customer)
        {
            //update status table product
            product.Status = 1;
            db.Entry(product).State = EntityState.Modified;

            //add thong tin table productactive
            if (userId.Length > 0)
            {
                var prodActive = new ProductActive()
                {
                    Activedate = DateTime.Now,
                    ProductId = product.Id,
                    CustomerId = customer,
                    Type = 2,
                    Activeby = userId,

                };
                db.ProductActives.Add(prodActive);
            }
            else
            {
                var prodActive = new ProductActive()
                {
                    Activedate = DateTime.Now,
                    ProductId = product.Id,
                    CustomerId = customer,
                    Type = 1,

                };
                db.ProductActives.Add(prodActive);
            }

            db.SaveChanges();
        }
        private void checkSendBrandname(string partner, string phone, string s1, string s2, string s3)
        {
            var brand = db.TempBrandnames.Find(partner);
            if (brand != null)
            {
                string mess = string.Format(brand.TempActive, brand.ShowName, s1, s2, s3);
                int send = Utils.SendMTBrandname.SentMTMessage(mess, phone, brand.ShowName, brand.ShowName, "0");
                Brandname brandName = new Brandname()
                {
                    Status = send,
                    Message = mess,
                    Createdate = DateTime.Now,
                    PhoneSend = phone
                };
                db.Brandnames.Add(brandName);
                db.SaveChanges();
            }
        }
        public List<Province> getProvince()
        {
            var province = db.Provinces.OrderBy(a => a.Name).ToList();
            return province;
        }

        [HttpPost]
        public ActionResult GetCity(string name)
        {
            District city = new District();
            var id = db.Provinces.Where(s => s.Name == name).SingleOrDefault();//get id theo ten
            var provi = db.Districts.Where(x => x.ProvinceId == id.Id).ToList();//get ds quan huyen
            var ress = new List<String>();//add data vao response
            foreach (var i in provi)
            {
                ress.Add(i.Name);
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(ress);//convert to json
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult resetActivation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult resetActivation(string serial)
        {
            serial = serial.Trim();
            var product = db.Products.Where(s => s.Serial == serial).SingleOrDefault();
            if (product == null)
            {
                return Json(ResResult("Mã cào đã nhập không tồn tại."), JsonRequestBehavior.AllowGet);
            }

            product.Status = null;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            var prodactive = db.ProductActives.Where(i => i.ProductId == product.Id).SingleOrDefault();
            db.ProductActives.Remove(prodactive);
            db.SaveChanges();

            return Json(ResResult("Sản phẩm với mã cào " + serial + " đã được reset kích hoạt thành công."), JsonRequestBehavior.AllowGet);
        }

        public string ResResult(string message)
        {
            ResetResult ress = new ResetResult()
            {
                message = message
            };
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(ress);//convert to json
            return result;
        }

        public class ResetResult
        {
            public string message { get; set; }
        }

        public ActionResult Uploadfile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Uploadfile(FormCollection formCollection)
        {
            ViewBag.mess = "Đã có lỗi xảy ra.";
            if (Request != null)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        //int count = 0;
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var products = new List<UploadProductModel>();
                        int count = 0;
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                UploadProductModel prodview = new UploadProductModel();
                                Product product = new Product();
                                product.Createdate = DateTime.Now;
                                //Vincent
                                product.Createby = Utility.IdPatner;
                                product.Serial = workSheet.Cells[rowIterator, 1].Value.ToString();
                                product.Name = workSheet.Cells[rowIterator, 2].Value.ToString();
                                product.Code = workSheet.Cells[rowIterator, 3].Value.ToString();

                                //product.Model = workSheet.Cells[rowIterator, 4].Value.ToString();
                                //Vincent - add try catch
                                try { product.Model = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { }

                                try { product.MadeIn = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { }

                                try
                                {
                                    product.Exportdate = DateTime.ParseExact(workSheet.Cells[rowIterator, 6].Value.ToString(), "dd/MM/yyyy", null);
                                }
                                catch (Exception) { }
                                try
                                {
                                    product.Arisingdate = DateTime.ParseExact(workSheet.Cells[rowIterator, 7].Value.ToString(), "dd/MM/yyyy", null);
                                }
                                catch (Exception) { }
                                product.Limited = int.Parse(workSheet.Cells[rowIterator, 8].Value.ToString());
                                try
                                {
                                    product.Category = workSheet.Cells[rowIterator, 9].Value.ToString();
                                }
                                catch (Exception) { }
                                try
                                {
                                    product.Importdate = DateTime.ParseExact(workSheet.Cells[rowIterator, 10].Value.ToString(), "dd/MM/yyyy", null);
                                }
                                catch (Exception) { }

                                prodview.Product = product;
                                products.Add(prodview);
                                db.Products.Add(product);
                                var cp = db.Products.Where(a => a.Serial == product.Serial).SingleOrDefault();
                                if (cp != null)
                                {
                                    count++;
                                    prodview.Error = "Serial đã bị trùng.";
                                }

                            }
                            if (count == 0)
                            {
                                ViewBag.mess = "Đã lưu sản phẩm thành công.";
                                db.SaveChanges();

                                logger.Info("Luu san pham thanh cong");
                            }
                            else
                            {
                                logger.Info("Luu san pham khong thanh cong");
                            }
                        }
                        return View(products);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.mess = ex;
                    return View();
                }
            }
            return View();
        }

        public FileResult ExportExc()
        {
            var stream = CreateExcelFileDetail(listExc);
            var buffer = stream as MemoryStream;
            return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".xlsx");
        }
        private Stream CreateExcelFileDetail(List<Product> list, Stream stream = null)
        {
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "OledPro";
                excelPackage.Workbook.Properties.Company = "OledPro";
                excelPackage.Workbook.Properties.Title = "Báo cáo";
                excelPackage.Workbook.Worksheets.Add("Product");
                var workSheet = excelPackage.Workbook.Worksheets[1];
                //workSheet.Cells[1, 1].LoadFromCollection(list, true, TableStyles.Dark9);
                BindingFormatForExcelDetail(workSheet, list);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        private void BindingFormatForExcelDetail(ExcelWorksheet worksheet, List<Product> listItems)
        {
            // Set default width cho tất cả column
            worksheet.DefaultColWidth = 25;
            // Tự động xuống hàng khi text quá dài
            worksheet.Cells.Style.WrapText = false;
            // Tạo header
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 1].AutoFitColumns(6);
            worksheet.Cells[1, 2].Value = "Tên sản phẩm";
            worksheet.Cells[1, 2].AutoFitColumns();
            worksheet.Cells[1, 3].Value = "Serial";
            worksheet.Cells[1, 3].AutoFitColumns();
            worksheet.Cells[1, 4].Value = "Code";
            worksheet.Cells[1, 4].AutoFitColumns(17);
            worksheet.Cells[1, 5].Value = "Ngày xuất kho";
            worksheet.Cells[1, 5].AutoFitColumns(11);
            worksheet.Cells[1, 6].Value = "Ngày sản xuất";
            worksheet.Cells[1, 6].AutoFitColumns(11);
            worksheet.Cells[1, 7].Value = "Bảo hành(tháng)";
            worksheet.Cells[1, 7].AutoFitColumns(11);
            worksheet.Cells[1, 8].Value = "Trạng thái";
            worksheet.Cells[1, 8].AutoFitColumns(11);



            // Lấy range vào tạo format cho range đó ở đây là từ A1 tới I1
            using (var range = worksheet.Cells["A1:H1"])
            {
                // Set PatternType
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                // Set Màu cho Background
                // range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xc6d9f1));
                range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                // Canh giữa cho các text
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //// Set Font cho text  trong Range hiện tại
                //range.Style.Font.SetFromFont(new Font("Arial", 10));
                //// Set Border
                //range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                //// Set màu cho Border
                //range.Style.Border.Bottom.Color.SetColor(Color.DarkBlue);

                //range.AutoFilter = true;
                range.Style.Font.Bold = true;
                range.Style.Font.Size = 10;
                range.Style.Font.Color.SetColor(Color.White);
            }

            // Đỗ dữ liệu từ list vào 
            for (int i = 0; i < listItems.Count; i++)
            {
                var item = listItems[i];
                var rowCur = i + 2;
                string headerRange = "A" + rowCur + ":H" + rowCur;
                worksheet.Cells[headerRange].Style.Font.Bold = false;
                worksheet.Cells[headerRange].Style.Font.Size = 11;
                worksheet.Cells[headerRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["C" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["D" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                worksheet.Cells[rowCur, 1].Value = i + 1;
                worksheet.Cells[rowCur, 2].Value = item.Name;
                worksheet.Cells[rowCur, 3].Value = item.Serial;
                worksheet.Cells[rowCur, 4].Value = item.Model;

                if (item.Exportdate != null)
                {
                    worksheet.Cells[rowCur, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[rowCur, 5].Value = item.Exportdate.Value.ToString("dd/MM/yyyy");
                }
                if (item.Arisingdate != null)
                {
                    worksheet.Cells[rowCur, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[rowCur, 6].Value = item.Arisingdate.Value.ToString("dd/MM/yyyy");
                }

                worksheet.Cells[rowCur, 7].Value = item.Limited;

                if (item.Status == 1)
                {

                    worksheet.Cells[rowCur, 8].Value = "Kích hoạt";
                }
                else
                {
                    worksheet.Cells[rowCur, 8].Value = "";
                }
            }
        }
    }
}