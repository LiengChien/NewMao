using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NewMao.Service.BaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewMao.WebApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        protected readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var result = new Result();
            result.Fail("系統出錯，請在試一次或者聯繫管理員!");

            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
