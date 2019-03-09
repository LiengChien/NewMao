using NewMao.Common.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Entity.Code
{
    public class CodeConditions : BaseConditions
    {
        [Column(Name = "code_type", WhereTypeDefine = Common.Enum.WhereClause.EqualTo)]
        public string CodeType { get; set; }
    }
}
