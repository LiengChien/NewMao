using NewMao.Entity.Code;
using NewMao.Interface.BaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Interface.Service
{
    public interface ICodeService
    {
        IResult GetCodeList(CodeConditions conditions);
    }
}
