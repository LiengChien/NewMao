using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Interface.BaseModel
{
    public interface IResult
    {
        bool Success { get; set; }
        string Message { get; set; }
        object Data { get; set; }
    }
}
