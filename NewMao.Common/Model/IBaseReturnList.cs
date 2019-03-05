using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Model
{
    public interface IBaseReturnList<T>
    {
        IEnumerable<T> Items { get; set; }
        int CurrentPage { get; set; }
        int TotalPages { get; set; }
        int TotalSize { get; set; }
    }
}
