using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewMao.Entity.Code;
using NewMao.Interface.Service;

namespace NewMao.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CodeController : ControllerBase
    {
        ICodeService _codeService;
        IHostingEnvironment _hostingEnvironment;

        public CodeController(ICodeService codeService
            , IHostingEnvironment hostingEnvironment)
        {
            _codeService = codeService;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route(""), HttpGet]
        public IActionResult GetCodeList(CodeConditions conditions)
        {
            return Ok(_codeService.GetCodeList(conditions));
        }
    }
}