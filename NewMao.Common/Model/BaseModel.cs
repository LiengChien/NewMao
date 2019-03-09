using NewMao.Common.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Model
{
    public class BaseModel : IBaseModel
    {
        [Column(Name = "created_date")]
        public virtual DateTime? CreatedDate { get; set; }

        [Column(Name = "created_by")]
        public virtual string CreatedBy { get; set; }

        [Column(Name = "updated_date")]
        public virtual DateTime? UpdatedDate { get; set; }

        [Column(Name = "updated_by")]
        public virtual string UpdatedBy { get; set; }

        [Column(Name = "active")]
        public virtual string Active { get; set; }

        [Column(Name = "deleted_date")]
        public virtual DateTime? DeletedDate { get; set; }

        [Column(Name = "deleted_by")]
        public virtual string DeletedBy { get; set; }

        [Column(Name = "module_name")]
        public virtual string ModuleName { get; set; }
    }
}
