using NewMao.Common.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Model
{
    public class BaseModel : IBaseModel
    {
        [Column(Name = "created_date")]
        public DateTime? CreatedDate { get; set; }

        [Column(Name = "created_by")]
        public string CreatedBy { get; set; }

        [Column(Name = "updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column(Name = "updated_by")]
        public string UpdatedBy { get; set; }

        [Column(Name = "active")]
        public string Active { get; set; }

        [Column(Name = "deleted_date")]
        public DateTime? DeletedDate { get; set; }

        [Column(Name = "deleted_by")]
        public string DeletedBy { get; set; }

        [Column(Name = "module_name")]
        public string MooduleName { get; set; }
    }
}
