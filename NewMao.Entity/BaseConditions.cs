using NewMao.Common.Mapper;
using NewMao.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Entity
{
    public class BaseConditions : BasePager
    {
        [Column(Name = "Updated_by", WhereTypeDefine = Common.Enum.WhereClause.In)]
        public List<string> DataMembers { get; set; }

        public string CurrentEmpId { get; set; }

        public string CurrentUserRoleIds { get; set; }

        [Column(Name = "module_name", WhereTypeDefine = Common.Enum.WhereClause.EqualTo)]
        public string ModuleName { get; set; }
    }
}
