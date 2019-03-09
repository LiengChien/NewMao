using NewMao.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Core.Extensions
{
    public static class ReturnListExtension
    {
        public static IBaseReturnList<T> GetBaseReturnList<T>(this IEnumerable<T> models, int totalSize, IBasePager pager) where T : class
        {
            var rtnList = new BaseReturnList<T>();
            rtnList.CurrentPage = pager.PageNum;
            rtnList.Items = models;
            rtnList.TotalSize = totalSize;
            rtnList.TotalPages = totalSize.GetTotalPages(pager.PageSize);
            return rtnList;
        }
    }
}
