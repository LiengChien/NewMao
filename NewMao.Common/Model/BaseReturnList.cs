using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Model
{
    public class BaseReturnList<T> : IBaseReturnList<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalSize { get; set; }
    }
}
