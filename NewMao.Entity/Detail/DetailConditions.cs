using NewMao.Common.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Entity.Detail
{
    public class DetailConditions : BaseConditions
    {
        [Column(Name = "master_id", WhereTypeDefine = Common.Enum.WhereClause.EqualTo)]
        public int? MasterId { get; set; }

        [Column(Name = "master_id", WhereTypeDefine = Common.Enum.WhereClause.In)]
        public List<int> MasterIds { get; set; }

        /// <summary>
        /// 識別對應DetailTable
        /// </summary>
        public string TableName { get; set; }
        
        [Column(Name = "module_name", WhereTypeDefine = Common.Enum.WhereClause.EqualTo)]
        public string ModuleName { get; set; } = null;

        public override bool NeedPager { get; set; } = false;
    }
}
