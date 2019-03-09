using NewMao.Entity.Detail;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Interface.Repository
{
    public interface IDetailRepository : IRepository<Detail ,DetailConditions ,int>
    {
    }
}
