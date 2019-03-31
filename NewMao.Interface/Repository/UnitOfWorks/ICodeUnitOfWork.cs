using NewMao.Entity.Code;
using NewMao.Entity.CodeMapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Interface.Repository.UnitOfWorks
{
    public interface ICodeUnitOfWork : IUnitOfWork
    {
        IRepository<Code,CodeConditions,int> CodeRepo { get; }

        IRepository<CodeMapping, CodeMappingConditions, int> CodeMappingRepo { get; }
    }
}
