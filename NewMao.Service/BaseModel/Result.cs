using NewMao.Interface.BaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Service.BaseModel
{
    public class Result : IResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public static class ResultExtension
    {
        /// <summary>
        /// 返回成功 （無文字訊息）
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static Result Success(this Result helper)
        {
            return helper.Success("");
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Success(this Result helper, string message)
        {
            helper.Success = true;
            helper.Message = message;
            return helper;
        }

        /// <summary>
        /// 返回失敗
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="resultMsg"></param>
        /// <returns></returns>
        public static Result Fail(this Result helper, string resultMsg)
        {
            helper.Success = false;
            helper.Message = resultMsg;
            return helper;
        }
    }
}
