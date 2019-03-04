using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NewMao.Common.Enum
{
    public enum AndOrClause
    {
        [Description("使用And組條件")]
        AndClause = 1,
        [Description("使用Or組條件")]
        OrClause = 2
    }
}
