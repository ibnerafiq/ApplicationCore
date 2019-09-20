using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.BaseService
{
    public enum ResultType
    {
        Success,
        Error,
        Empty,
        ValidationErrors,
        Exception,
        Failure,
        AutheticationError
    }
}
