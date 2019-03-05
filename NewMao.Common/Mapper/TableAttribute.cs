using NewMao.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Mapper
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public SelectType TableSelectType { get; set; } = SelectType.Common;
        public bool NeedDistinct { get; set; } = false;
    }
}
