using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BHDT_OledPro.Utils
{
    public class FormatString
    {
        public static String getMobileOperator(String mobileNumber)
        {
            if (mobileNumber == null || mobileNumber == "")
            {
                return "CITY";
            }
            if (mobileNumber.StartsWith("+"))
            {
                mobileNumber = mobileNumber.Substring(1);
            }
            if (!mobileNumber.StartsWith("84") || (mobileNumber.StartsWith("84") && mobileNumber.Length == 9))
            {
                mobileNumber = formatUserId(mobileNumber, 0);
            }
            if (mobileNumber.StartsWith("8498") || mobileNumber.StartsWith("8497")
                    || mobileNumber.StartsWith("8496")
                    || mobileNumber.StartsWith("8486") || mobileNumber.StartsWith("8432")
                    || mobileNumber.StartsWith("8433") || mobileNumber.StartsWith("8434")
                    || mobileNumber.StartsWith("8435")
                    || mobileNumber.StartsWith("8436") || mobileNumber.StartsWith("8437")
                    || mobileNumber.StartsWith("8438") || mobileNumber.StartsWith("8439")
                    )
            {
                return "VIETEL";
            }
            else if (mobileNumber.StartsWith("8490") || mobileNumber.StartsWith("8493")
                  || mobileNumber.StartsWith("8489")
                  || mobileNumber.StartsWith("8470") || mobileNumber.StartsWith("8476")
                  || mobileNumber.StartsWith("8477") || mobileNumber.StartsWith("8478")
                  || mobileNumber.StartsWith("8479")
                  )
            {
                return "VMS";
            }
            else if (mobileNumber.StartsWith("8491") || mobileNumber.StartsWith("8494")
                  || mobileNumber.StartsWith("8488")
                  || mobileNumber.StartsWith("8481") || mobileNumber.StartsWith("8482")
                  || mobileNumber.StartsWith("8483") || mobileNumber.StartsWith("8484")
                  || mobileNumber.StartsWith("8485") || mobileNumber.StartsWith("8488")
                  )
            {
                return "GPC";
            }
            else if (mobileNumber.StartsWith("8492") || mobileNumber.StartsWith("8456")
                 || mobileNumber.StartsWith("8458") || mobileNumber.StartsWith("8452")
                 )
            {
                return "VNM";
            }
            else if (mobileNumber.StartsWith("8499")
                 || mobileNumber.StartsWith("8459"))
            {
                return "GTEL";


            }

            else
            {
                return "UNKNOWN";
            }
        }
        public static String formatUserId(String userId, int formatType)
        {
            //bool check = IsPhoneNumber(userId);
            //if (userId == null || "".Equals(userId) || check == false)
            //{
            //    return null;
            //}
            String temp = userId;
            switch (formatType)
            {
                case 0://Constants.USERID_FORMAT_INTERNATIONAL:
                    if ((temp.StartsWith("9") || temp.StartsWith("8") || temp.StartsWith("7") || temp.StartsWith("5") || temp.StartsWith("3")) && temp.Length == 9)
                    {
                        temp = "84" + temp;
                    }
                    else if (temp.StartsWith("1") && temp.Length == 10)
                    {
                        temp = "84" + temp;
                    }
                    else if (temp.StartsWith("0"))
                    {
                        temp = "84" + temp.Substring(1);
                    } // els  
                    break;
                case 1://Constants.USERID_FORMAT_NATIONAL_NINE:
                    if (temp.StartsWith("84"))
                    {
                        temp = temp.Substring(2);
                    }
                    else if (temp.StartsWith("0"))
                    {
                        temp = temp.Substring(1);
                    } // else startsWith("9")
                    break;
                case 2://Constants.USERID_FORMAT_NATIONAL_ZERO:
                    if (temp.StartsWith("84"))
                    {
                        temp = "0" + temp.Substring(2);
                    }
                    else if (!temp.StartsWith("0"))
                    {
                        temp = "0" + temp;
                    } // else startsWith("09")
                    break;
                default:

                    return null;
            }
            return temp;
        }
        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+[0-9]{9})$").Success;
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}