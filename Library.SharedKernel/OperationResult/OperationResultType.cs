using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.SharedKernel.OperationResult
{
    public enum OperationResultType
    {
        Success = 1,
        Exist = 2,
        NotExist = 3,
        Failed = 4,
        Exception = 5,
        Forbidden = 6,
        Unauthorized = 7,
    }
}
