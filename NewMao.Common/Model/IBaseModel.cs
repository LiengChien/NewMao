using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Common.Model
{
    public interface IBaseModel
    {
        DateTime? CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }
        string Active { get; set; }
        DateTime? DeletedDate { get; set; }
        string DeletedBy { get; set; }
        string MooduleName { get; set; }
    }
}
