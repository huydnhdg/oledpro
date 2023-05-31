using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHDT_OledPro.Utils
{
    public class Utility
    {
        public static Dictionary<string, string> openWith = new Dictionary<string, string>(){
                                        {"oled_c1s", "Oled C1s"},
                                        {"oled_c2", "Oled C2"},
                                        {"oled_c8", "Oled C8"},
                                        {"oledpro_x3", "OledPro X3"},
                                        {"oledpro_x5", "OledPro X5"},
                                        {"oledpro_c8s", "OledPro C8s"},
                                        {"oledpro_c3s", "OledPro C3s"},
                                        {"oledpro_c5s", "OledPro C5s"}
                                    };
        

        public static string getclientIP()
        {
            var HostIP = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
            return HostIP;
        }
        public static string IdPatner = "517e5486-cb22-4b7f-a398-99ed96e32231";


        public static string getProductCode(string rawProductCode)
        {
            string productCode = "";
            switch (rawProductCode)
            {
                case "oled_c1":
                    productCode = "Oled C1";
                    break;
                case "oled_c1s":
                    productCode = "Oled C1s";
                    break;
                case "oled_c2":
                    productCode = "Oled C2";
                    break;
                case "oled_c8":
                    productCode = "Oled C8";
                    break;
                case "oledpro_x3":
                    productCode = "OledPro X3";
                    break;
                case "oledpro_x5":
                    productCode = "OledPro X5";
                    break;
                case "oledpro_c8s":
                    productCode = "OledPro C8s";
                    break;
                case "oledpro_c3s":
                    productCode = "OledPro C3s";
                    break;
                case "oledpro_c5s":
                    productCode = "OledPro C5s";
                    break;
                default:
                    productCode = "";
                    break;
            }

            return productCode;
        }
    }
}