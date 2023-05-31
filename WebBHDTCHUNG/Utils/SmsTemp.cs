using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHDT_OledPro.Utils
{
    public class SmsTemp
    {
        //
        public static string WARRANTI()
        {
            return string.Format("Yeu cau bao hanh cua ban da duoc ghi nhan, chung toi se lien he lai trong vong 24h. Cam on QK");
        }
        //
        public static string ACTIVE_NOTVALID()
        {
            return string.Format("So serial cua ban khong ton tai. Vui long kiem tra lai serial hoac truy cap http://kichhoat.baohanhdientu.vip/");
        }
        public static string ACTIVE(string s1, string s2, string s3, string s4)
        {
            return string.Format("San pham {0} serial {1} da kich hoat thanh cong tu ngay {2} den ngay {3}. Cam on QK", s1, s2, s3, s4);
        }
        //
        public static string TRACUU_NOTVALID()
        {
            return string.Format("So serial cua ban khong ton tai. Vui long kiem tra lai serial hoac truy cap http://kichhoat.baohanhdientu.vip/");
        }
        public static string TRACUU_ACTIVE(string s1, string s2, string s3, string s4)
        {
            return string.Format("San pham {0} serial {1} da kich hoat thanh cong tu ngay {2} den ngay {3}. Cam on QK", s1, s2, s3, s4);
        }
        public static string TRACUU_NOACTIVE(string s1, string s2, string s3)
        {
            return string.Format("San pham {0} serial {1} chua duoc kich hoat. Vui long soan {2} KH [serial] gui 8077 hoac truy cap http://kichhoat.baohanhdientu.vip/", s1, s2, s3);
        }
        //
    }
}