using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NewMao.Core.Extensions
{
    public static class CommonExtension
    {
        public static string GetContentType(this string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        
        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
              {".txt", "text/plain"},
              {".pdf","application/pdf"},
              {".doc","application/vnd.ms-word"},
              {".docx","application/vnd.ms-word"},
              {".xls","application/vnd.ms-excel"},
              {".xlsx","application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
              {".png","image/png"},
              {".jpg","image/jpeg"},
              {".jpeg","image/jpeg"},
              {".gif","image/gif"},
              {".csv","text/csv"}
            };
        }

        public static string ToMoney(this decimal num, bool isShowZero = true)
        {
            if (num == 0)
            {
                return string.Empty;
            }
            return num.ToString("#,0");
        }

        public static string ToTwdCharacter(this decimal num)
        {
            string[] cstr = { "零", "壹", "貳", "叁", "肆", "伍", "陸", "柒", "捌", "玖" };
            string[] wstr = { "", "", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "億", "拾", "佰", "仟" };
            string str = num.ToString();
            int len = str.Length;
            int i;
            string tmpstr, rstr;

            rstr = "";

            for (i = 1; i <= len; i++)
            {
                tmpstr = str.Substring(len - i, 1);
                rstr = string.Concat(cstr[Int32.Parse(tmpstr)] + wstr[i], rstr);
            }

            rstr = rstr.Replace("拾零", "拾");

            rstr = rstr.Replace("零拾", "零");

            rstr = rstr.Replace("零佰", "零");

            rstr = rstr.Replace("零仟", "零");

            rstr = rstr.Replace("零萬", "萬");

            for (i = 1; i <= 6; i++)

                rstr = rstr.Replace("零零", "零");

            rstr = rstr.Replace("零萬", "零");

            rstr = rstr.Replace("零億", "億");

            rstr = rstr.Replace("零零", "零");

            return rstr;
        }
    }
}
