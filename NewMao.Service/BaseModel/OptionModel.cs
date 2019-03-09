using NewMao.Interface.BaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Service.BaseModel
{
    public class OptionModel : IOptionModel
    {
        public string Key { get; set; }
        public string Note { get; set; }
    }
}
