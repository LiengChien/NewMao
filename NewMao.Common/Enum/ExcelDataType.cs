using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NewMao.Common.Enum
{
    public enum ExcelDataType
    {
        [Description("數量類別")]
        QtyDecimals = 1,
        [Description("價格類別")]
        PriceDecimals = 2,
        [Description("日期時間 yyyy/mm/dd hh:mm:ss")]
        DateTime = 3,
        [Description("日期 yyyy/mm/dd")]
        Date = 4
    }
}
