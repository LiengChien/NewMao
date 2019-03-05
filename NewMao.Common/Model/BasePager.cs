using NewMao.Common.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Model
{
    public class BasePager : IBasePager
    {
        public int PageSize { get; set; } = 10;
        public int PageNum { get; set; } = 1;
        public int Offset
        {
            get
            {
                return PageSize * (PageNum - 1);
            }
        }
        public virtual bool NeedPager { get; set; } = true;
    }
}
