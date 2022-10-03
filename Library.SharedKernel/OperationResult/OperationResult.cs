using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.SharedKernel.OperationResult
{
    public class OperationResult<T>
    {
        public OperationResultType OperationResultType { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public OperationResult<T> SetSuccess(T result)
        {
            OperationResultType = OperationResultType.Success;
            Result = result;
            return this;
        }

        public OperationResult<T> SetFailed(string message, OperationResultType type = OperationResultType.Failed)
        {
            OperationResultType = type;
            Message = message;
            return this;
        }

        public OperationResult<T> SetException(Exception exception)
        {
            OperationResultType = OperationResultType.Exception;
            Exception = exception;
            Message = exception.Message;
            return this;
        }


    }
}
