using NewMao.Common.Mapper;
using NewMao.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Entity.Detail
{
    [Table(Name = "$detail$")]
    public class Detail : BaseModel
    {
        [Column(Name = "detail_id")]
        public int? DetailId { get; set; }

        [Column(Name = "master_id", IsKey = true)]
        public int MasterId { get; set; }

        [Column(Name = "code_type")]
        public string CodeType { get; set; }

        [Column(Name = "code_id")]
        public string CodeId { get; set; }

        [Column(Name = "value")]
        public string Value { get; set; }

        [Column(Name = "column_name")]
        public string ColumnName { get; set; }

        public string TableName { get; set; }

        [Column(Name = "module_name", IsKey = true)]
        public override string ModuleName { get => base.ModuleName; set => base.ModuleName = value; }

        [Column(Name = "active")]
        public override string Active { get; set; } = null;
    }
}
