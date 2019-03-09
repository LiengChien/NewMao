using NewMao.Entity.Portal;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Interface.BaseModel
{
    public interface IPermission
    {
        IEnumerable<ApiPermission> ApiPermissionList { get; }
    }
}
