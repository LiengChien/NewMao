using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Model
{
    public interface IBasePager
    {
        int PageSize { get; set; }
        int PageNum { get; set; }
        int Offset { get; }
        bool NeedPager { get; set; }
    }
}
