using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NewMao.Core.Extensions
{
    public class Extensions
    {
        public static string ReadFile(string filePath)
        {
            string content = string.Empty;

            using (StreamReader sr = new StreamReader(filePath))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }
    }
}
