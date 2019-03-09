using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Core.Extensions
{
    public static class PagerExtension
    {
        public static int GetTotalPages(this int totalSize, int pageSize)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalSize / (decimal)pageSize);
            totalPages += totalPages == 0 ? (totalSize > 0 ? 1 : 0) : 0;
            return totalPages;
        }
    }
}
