using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHDT_OledPro.Models
{
    public class ProductActiveModel:Product
    {
        public Nullable<DateTime> Activedate { get; set; }
        public Nullable<DateTime> Buydate { get; set; }
        public String ProductCode { get; set; }
        public String Car_Brandname { get; set; }
        public String CustomerName { get; set; }
        public String CustomerPhone { get; set; }
        public String CustomerAddress { get; set; }
        public String CustomerProvince { get; set; }
        public string AgentAddress { get; set; }
    }
}