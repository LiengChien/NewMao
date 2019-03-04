using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Enum
{
    public enum WhereClause
    {
        EqualTo = 0,
        Like = 1,
        LikeStartWith = 2,
        LikeEndWith = 3,
        Between = 4,
        GreaterThan = 5,
        LessThan = 6,
        GreaterThanOrEqualTo = 7,
        LessThanOrEqualTo = 8,
        In = 9,
        NotEqualTo = 10,
        BetweenWithTwoColumns = 11
    }
}
