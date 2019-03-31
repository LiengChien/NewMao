using NewMao.Common.Mapper;
using NewMao.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Entity.CodeMapping
{
    [Table(Name = "code_mapping")]
    public class CodeMapping : BaseModel
    {
        [Column(Name = "id", IsKey = true)]
        public int Id { get; set; }

        [Column(Name = "module_name")]
        public new string ModuleName { get; set; }

        [Column(Name = "code_type")]
        public string CodeType { get; set; }

        [Column(Name = "modified")]
        public bool? Modified { get; set; }
    }
}
