using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.SharedKernel.OperationResult
{
    public static class OperationResultExtensions
    {
        public static JsonResult ToJsonResult<T>(this OperationResult<T> result)
        {
            return result.OperationResultType switch
            {
                OperationResultType.Success => new JsonResult(result.Result) { StatusCode = StatusCodes.Status200OK },
                OperationResultType.Exist => new JsonResult(result.OperationResultType.ToString()) { StatusCode = StatusCodes.Status202Accepted },
                OperationResultType.NotExist => new JsonResult(result.Message.ToString()) { StatusCode = StatusCodes.Status404NotFound },
                OperationResultType.Failed => new JsonResult(result.Message) { StatusCode = StatusCodes.Status400BadRequest },
                OperationResultType.Forbidden => new JsonResult(result.Message) { StatusCode = StatusCodes.Status403Forbidden },
                OperationResultType.Unauthorized => new JsonResult(result.Message) { StatusCode = StatusCodes.Status401Unauthorized },
                OperationResultType.Exception => new JsonResult(result.Exception) { StatusCode = StatusCodes.Status500InternalServerError },
                _ => new JsonResult(string.Empty),
            };
        }

        public static async Task<JsonResult> ToJsonResultAsync<T>(this Task<OperationResult<T>> result)
            => (await result).ToJsonResult();
    }
}
