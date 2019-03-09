using Microsoft.Extensions.Configuration;
using NewMao.Entity.Code;
using NewMao.Interface.Repository;
using NewMao.Interface.Repository.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Repository.UnitOfWork
{
    public class CodeUnitOfWork : GenericUnitOfWork, ICodeUnitOfWork
    {
        private IRepository<Code, CodeConditions, int> _codeRepo;

        public CodeUnitOfWork(IConfiguration config) : base(config) { }

        public IRepository<Code, CodeConditions, int> CodeRepo => throw new NotImplementedException();
    }
}
