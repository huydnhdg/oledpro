using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BHDT_OledPro.Models;

namespace BHDT_OledPro.Areas.Admin.Data
{
    public class ProductAgentModel 
    {
        public string ProdName { get; set; }
        public string Agent { get; set; }
        public string Importdate { get; set; }

        public string Error { get; set; }
    }
}