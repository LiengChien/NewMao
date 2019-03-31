using NewMao.Common.Mapper;
using NewMao.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Entity.Code
{
    [Table(Name = "code", TableSelectType = Common.Enum.SelectType.Complicated)]
    public class Code : BaseModel
    {
        [Column(Name = "id", IsKey = true)]
        public int Id { get; set; }

        [Column(Name = "code_type")]
        public string CodeType { get; set; }

        [Column(Name = "code_id")]
        public string CodeId { get; set; }

        [Column(Name = "code_name")]
        public string CodeName { get; set; }

        [Column(Name = "code_name_note")]
        public string CodeNameNote { get; set; }

        [Column(Name = "enable")]
        public bool? Enable { get; set; }

        [Column(Name = "order")]
        public int? Order { get; set; }
    }
}
