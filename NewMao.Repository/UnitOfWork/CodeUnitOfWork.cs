using Microsoft.Extensions.Configuration;
using NewMao.Entity.Code;
using NewMao.Entity.CodeMapping;
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
        private IRepository<CodeMapping, CodeMappingConditions, int> _codeMappingRepo;

        public CodeUnitOfWork(IConfiguration config) : base(config) { }

        public IRepository<Code, CodeConditions, int> CodeRepo => _codeRepo ?? (_codeRepo = new GenericRepository<Code, CodeConditions, int>(_transaction));

        public IRepository<CodeMapping, CodeMappingConditions, int> CodeMappingRepo => _codeMappingRepo ?? (_codeMappingRepo = new GenericRepository<CodeMapping, CodeMappingConditions, int>(_transaction));
    }
}
