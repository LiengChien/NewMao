using NewMao.Entity.Code;
using NewMao.Entity.CodeMapping;
using NewMao.Interface.BaseModel;
using NewMao.Interface.Repository.UnitOfWorks;
using NewMao.Interface.Service;
using NewMao.Service.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewMao.Service.Services
{
    class CodeService : ICodeService
    {
        ICodeUnitOfWork _codeUnitOfWork;

        public CodeService(ICodeUnitOfWork codeUnitOfWork)
        {
            _codeUnitOfWork = codeUnitOfWork;
        }

        public IResult GetCodeList(CodeConditions conditions)
        {
            var result = new Result();

            var totalSize = 0;
            if (!string.IsNullOrEmpty(conditions.ModuleName))
            {
                var frmCodeTypeList = _codeUnitOfWork.CodeMappingRepo.GetAll(
                    new CodeMappingConditions
                    {
                        ModuleName = conditions.ModuleName
                    }, out totalSize);
                if (frmCodeTypeList.Count() > 0)
                {
                    conditions.CodeType = string.Join(",", frmCodeTypeList.Select(x => x.CodeType).ToArray<string>());
                }
            }
            var codeList = _codeUnitOfWork.CodeRepo.GetAll(conditions, out totalSize);
            var codeDict = new Dictionary<string, List<object>>();
            var codeTypeList = codeList.Select(x => x.CodeType).Distinct();
            foreach (var item in codeTypeList)
            {
                var codeListWithType = codeList.Where(x => x.CodeType == item).OrderBy(x => x.Order).ToList();
                //for vue-select
                var tempList = new List<object>();
                foreach (var code in codeListWithType)
                {
                    tempList.Add(new
                    {
                        label = code.CodeName,
                        value = code.CodeId
                    });
                }
                codeDict.Add(item, tempList);

                var tempListOrg = new List<object>();
                foreach (var code in codeListWithType)
                {
                    tempListOrg.Add(new
                    {
                        id = code.Id,
                        code_type = code.CodeType,
                        code_id = code.CodeId,
                        code_name = code.CodeName,
                        code_name_note = code.CodeNameNote,
                        order = code.Order
                    });
                }
                codeDict.Add($"{item}_ORG", tempListOrg);
            }
            result.Data = codeDict;
            result.Success();

            return result;
        }
    }
}
