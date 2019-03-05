using NewMao.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Mapper
{
    public class ExcelAttribute : Attribute
    {
        public string Name { get; set; }
        public int ListOrder { get; set; }
        public Boolean NumberFormat { get; set; } = false;
        public ExcelDataType DataType { get; set; } = 0;
    }
}
