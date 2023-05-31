using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BHDT_OledPro.Areas.Admin.Data;
using BHDT_OledPro.Models;
using BHDT_OledPro.Utils;
using PagedList;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BHDT_OledPro.Areas.Admin.Controllers
{
    [Authorize]
    public class ActiveController : Controller
    {
        private CMSBHDTCHUNGEntities db = new CMSBHDTCHUNGEntities();
        private static List<ActiveViewModel> listExc = new List<ActiveViewModel>();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string startDate, string endDate, string currentStart, string currentEnd, string sdate, string edate, string currentS, string currentE, string status, string currentStatus, int? channel)
        {
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


            if (User.Identity.Name == "administrator")
            {
                var model = from a in db.ProductActives
                            join b in db.Products on a.ProductId equals b.Id
                            join c in db.Customers on a.CustomerId equals c.Id
                            select new ActiveViewModel()
                            {
                                Id = a.Id,
                                Activedate = a.Activedate,
                                Activeby = a.Activeby,
                                Type = a.Type,
                                Buydate = a.Buydate,
                                ProductName = b.Name,
                                Limited = b.Limited,
                                Serial = b.Serial,
                                CustomerName = c.Name,
                                CustomerPhone = c.Phone,
                                CustomerId = c.Id,
                                CustomerAddress = c.Address,
                                CustomerDistrict = c.District,
                                CustomerCity = c.City,
                                InstallationAgentAddress = a.InstallationAgentAddress,
                                CarBrandname = a.CarBrandname,
                                ProductCode = a.ProductCode
                            };

                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.ProductName.Contains(searchString)
                                           || s.Serial.Contains(searchString)
                                           || s.CustomerPhone.Contains(searchString)
                                           || s.Activeby.Contains(searchString)
                                           );
                }

                if (!String.IsNullOrEmpty(startDate))
                {
                    DateTime d = DateTime.Parse(startDate);
                    model = model.OrderByDescending(a => a.Activedate).Where(s => s.Activedate >= d);
                }
                if (!String.IsNullOrEmpty(endDate))
                {
                    DateTime d = DateTime.Parse(endDate);
                    d = d.AddDays(1);
                    model = model.OrderByDescending(a => a.Activedate).Where(s => s.Activedate <= d);
                }


                if (channel != null)
                {
                    model = model.Where(c => c.Type == channel);
                    ViewBag.Channel = channel;
                }
                model = model.OrderByDescending(a => a.Activedate);

                listExc = model.ToList();
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(listExc.ToPagedList(pageNumber, pageSize));


                // return View(model);
            }
            // string userId = User.Identity.GetUserId();
            var model1 = from a in db.ProductActives
                         join b in db.Products on a.ProductId equals b.Id
                         where b.Createby == Utility.IdPatner //Vincent
                         join c in db.Customers on a.CustomerId equals c.Id
                         select new ActiveViewModel()
                         {
                             Id = a.Id,
                             Activedate = a.Activedate,
                             Activeby = a.Activeby,
                             Type = a.Type,
                             Buydate = a.Buydate,
                             ProductName = b.Name,
                             Limited = b.Limited,
                             Serial = b.Serial,
                             CustomerName = c.Name,
                             CustomerPhone = c.Phone,
                             CustomerId = c.Id,
                             CustomerAddress = c.Address,
                             CustomerDistrict = c.District,
                             CustomerCity = c.City,
                             InstallationAgentAddress = a.InstallationAgentAddress,
                             CarBrandname = a.CarBrandname,
                             ProductCode = a.ProductCode
                         };
            if (!String.IsNullOrEmpty(searchString))
            {
                model1 = model1.Where(s => s.ProductName.Contains(searchString)
                                       || s.Serial.Contains(searchString)
                                       || s.CustomerPhone.Contains(searchString)
                                       || s.Activeby.Contains(searchString)
                                       );
            }
            if (!String.IsNullOrEmpty(startDate))
            {
                DateTime d = DateTime.Parse(startDate);
                model1 = model1.OrderByDescending(a => a.Activedate).Where(s => s.Activedate >= d);
            }
            if (!String.IsNullOrEmpty(endDate))
            {
                DateTime d = DateTime.Parse(endDate);
                d = d.AddDays(1);
                model1 = model1.OrderByDescending(a => a.Activedate).Where(s => s.Activedate <= d);
            }

            if (channel != null)
            {
                model1 = model1.Where(c => c.Type == channel);
                ViewBag.Channel = model1;
            }
            model1 = model1.OrderByDescending(a => a.Activedate);

            listExc = model1.ToList();
            int pageSize2 = 10;
            int pageNumber2 = (page ?? 1);
            return View(listExc.ToPagedList(pageNumber2, pageSize2));
            // return View(model1);
        }

        public ActionResult ListOfCustomer(long? id)
        {

            if (User.Identity.Name == "administrator")
            {
                var model1 = from a in db.ProductActives
                             join b in db.Products on a.ProductId equals b.Id
                             join c in db.Customers on a.CustomerId equals c.Id
                             select new ActiveViewModel()
                             {
                                 Id = a.Id,
                                 Activedate = a.Activedate,
                                 Activeby = a.Activeby,
                                 Type = a.Type,
                                 Buydate = a.Buydate,
                                 ProductName = b.Name,
                                 Limited = b.Limited,
                                 Serial = b.Serial,
                                 CustomerName = c.Name,
                                 CustomerPhone = c.Phone,
                                 CustomerId = c.Id,
                                 CustomerAddress = c.Address,
                                 CustomerDistrict = c.District,
                                 CustomerCity = c.City,
                                 InstallationAgentAddress = a.InstallationAgentAddress,
                                 CarBrandname = a.CarBrandname,
                                 ProductCode = a.ProductCode
                             };
                if (id != null)
                {
                    model1 = model1.Where(a => a.CustomerId == id);
                }
                return View(model1);
            }
            string userId = User.Identity.GetUserId();
            var model = from a in db.ProductActives
                        join b in db.Products on a.ProductId equals b.Id
                        where b.Createby == Utility.IdPatner //Vincent
                        join c in db.Customers on a.CustomerId equals c.Id
                        select new ActiveViewModel()
                        {
                            Id = a.Id,
                            Activedate = a.Activedate,
                            Activeby = a.Activeby,
                            Type = a.Type,
                            Buydate = a.Buydate,
                            ProductName = b.Name,
                            Limited = b.Limited,
                            Serial = b.Serial,
                            CustomerName = c.Name,
                            CustomerPhone = c.Phone,
                            CustomerId = c.Id,
                            CustomerAddress = c.Address,
                            CustomerDistrict = c.District,
                            CustomerCity = c.City,
                            InstallationAgentAddress = a.InstallationAgentAddress,
                            CarBrandname = a.CarBrandname,
                            ProductCode = a.ProductCode
                        };
            if (id != null)
            {
                model = model.Where(a => a.CustomerId == id);
            }


            return View(model);
        }

        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();
            ViewBag.ProductId = new SelectList(db.Products.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            ViewBag.CustomerId = new SelectList(db.Customers.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] ProductActive productActive)
        {
            if (ModelState.IsValid)
            {
                db.ProductActives.Add(productActive);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productActive);

        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductActive productActive = db.ProductActives.Find(id);
            if (productActive == null)
            {
                return HttpNotFound();
            }
            //Vincent
            //string userId = User.Identity.GetUserId();
            string userId = Utility.IdPatner;
            productActive.ProductName = db.Products.Where(a => a.Createby == userId && a.Id == productActive.ProductId).FirstOrDefault().Name;
            productActive.CustomerName = db.Customers.Where(a => a.Createby == userId && a.Id == productActive.CustomerId).FirstOrDefault().Name;
            // ViewBag.ProductId = new SelectList(db.Products.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            // ViewBag.ProductCode = new SelectList(Utility.openWith.ToList(), "Key", "Value");
            // ViewBag.CustomerId = new SelectList(db.Customers.Where(a => a.Createby == userId).ToList(), "Id", "Name", null);
            // productActive.InstallationAgentAddress = customer.InstallationAgentAddress;
            // productActive.CarBrandname = customer.CarBrandname;
            return View(productActive);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] ProductActive productActive)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productActive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productActive);
        }
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductActive productActive = db.ProductActives.Find(id);
            if (productActive == null)
            {
                return HttpNotFound();
            }
            return View(productActive);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProductActive productActive = db.ProductActives.Find(id);
            db.ProductActives.Remove(productActive);

            var prod = db.Products.Find(productActive.ProductId);
            prod.Status = null;
            db.Entry(prod).State = EntityState.Modified;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public FileResult ExportExc()
        {
            var stream = CreateExcelFileDetail(listExc);
            var buffer = stream as MemoryStream;
            return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "active_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".xlsx");
        }
        private Stream CreateExcelFileDetail(List<ActiveViewModel> list, Stream stream = null)
        {
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "OledPro";
                excelPackage.Workbook.Properties.Company = "OledPro";
                excelPackage.Workbook.Properties.Title = "Báo cáo";
                excelPackage.Workbook.Worksheets.Add("Active");
                var workSheet = excelPackage.Workbook.Worksheets[1];
                //workSheet.Cells[1, 1].LoadFromCollection(list, true, TableStyles.Dark9);
                BindingFormatForExcelDetail(workSheet, list);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        private void BindingFormatForExcelDetail(ExcelWorksheet worksheet, List<ActiveViewModel> listItems)
        {
            // Set default width cho t?t c? column
            worksheet.DefaultColWidth = 25;
            // T? d?ng xu?ng hàng khi text quá dài
            worksheet.Cells.Style.WrapText = false;
            // T?o header
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 1].AutoFitColumns(6);
            worksheet.Cells[1, 2].Value = "Tên sản phẩm";
            worksheet.Cells[1, 2].AutoFitColumns();
            worksheet.Cells[1, 3].Value = "Serial";
            worksheet.Cells[1, 3].AutoFitColumns(11);
            worksheet.Cells[1, 4].Value = "Tên khách hàng";
            worksheet.Cells[1, 4].AutoFitColumns(20);
            worksheet.Cells[1, 5].Value = "Số điện thoại";
            worksheet.Cells[1, 5].AutoFitColumns(11);
            worksheet.Cells[1, 6].Value = "Địa chỉ";
            worksheet.Cells[1, 6].AutoFitColumns();
            worksheet.Cells[1, 7].Value = "Tỉnh thành";
            worksheet.Cells[1, 7].AutoFitColumns(16);
            worksheet.Cells[1, 8].Value = "Đại lý lắp đặt";
            worksheet.Cells[1, 8].AutoFitColumns();

            worksheet.Cells[1, 9].Value = "Hãng xe/Đời xe";
            worksheet.Cells[1, 9].AutoFitColumns(11);
            worksheet.Cells[1, 10].Value = "Mã sản phẩm";
            worksheet.Cells[1, 10].AutoFitColumns(11);
            worksheet.Cells[1, 11].Value = "Ngày kích hoạt";
            worksheet.Cells[1, 11].AutoFitColumns(11);
            worksheet.Cells[1, 12].Value = "Ngày hết hạn";
            worksheet.Cells[1, 12].AutoFitColumns(11);
            worksheet.Cells[1, 13].Value = "Người kích hoạt";
            worksheet.Cells[1, 13].AutoFitColumns(11);
            worksheet.Cells[1, 14].Value = "Kênh";
            worksheet.Cells[1, 14].AutoFitColumns(10);



            // L?y range vào t?o format cho range dó ? dây là t? A1 t?i I1
            using (var range = worksheet.Cells["A1:N1"])
            {
                // Set PatternType
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                // Set Màu cho Background
                // range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xc6d9f1));
                range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                // Canh gi?a cho các text
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //// Set Font cho text  trong Range hi?n t?i
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

            // Ð? d? li?u t? list vào 
            for (int i = 0; i < listItems.Count; i++)
            {
                var item = listItems[i];
                var rowCur = i + 2;
                string headerRange = "A" + rowCur + ":N" + rowCur;
                worksheet.Cells[headerRange].Style.Font.Bold = false;
                worksheet.Cells[headerRange].Style.Font.Size = 11;
                worksheet.Cells[headerRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["K" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["L" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["N" + rowCur].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheet.Cells[rowCur, 1].Value = i + 1;
                worksheet.Cells[rowCur, 2].Value = item.ProductName;
                worksheet.Cells[rowCur, 3].Value = item.Serial;
                worksheet.Cells[rowCur, 4].Value = item.CustomerName;
                worksheet.Cells[rowCur, 5].Value = item.CustomerPhone;
                worksheet.Cells[rowCur, 6].Value = item.CustomerAddress;
                worksheet.Cells[rowCur, 7].Value = item.CustomerCity;
                worksheet.Cells[rowCur, 8].Value = item.InstallationAgentAddress;
                worksheet.Cells[rowCur, 9].Value = item.CarBrandname;
                worksheet.Cells[rowCur, 10].Value = item.ProductCode;

                if (item.Activedate != null)
                {
                    worksheet.Cells[rowCur, 11].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[rowCur, 11].Value = item.Activedate.Value.ToString("dd/MM/yyyy");
                }

                if (item.Activedate != null)
                {
                    DateTime datetime = item.Activedate.GetValueOrDefault().AddMonths(item.Limited.Value);
                    worksheet.Cells[rowCur, 12].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[rowCur, 12].Value = datetime.ToString("dd/MM/yyyy");
                }

                worksheet.Cells[rowCur, 13].Value = item.Activeby;

                if ((item.Type == 1) || (item.Type == 2))
                {
                    worksheet.Cells[rowCur, 14].Value = "website";
                }
                else
                {
                    worksheet.Cells[rowCur, 14].Value = "sms";
                }
            }
        }
    }
}