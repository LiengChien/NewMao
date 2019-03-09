using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Interface.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}
