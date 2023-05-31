using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BHDT_OledPro.Models;

namespace BHDT_OledPro.Areas.Admin.Data
{
    public class UploadProductModel : Product
    {
        public string Error { get; set; }
        public Product Product { get; set; }
    }
}