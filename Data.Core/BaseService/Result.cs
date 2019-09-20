using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.BaseService
{
    public class Result<T>
    {
        public ResultType ResultType { get; set; }

        public string TextType { get { return this.ResultType.ToString(); } }

        public string Message { get; set; }

        public T Data { get; set; }

        public List<ValidationError> ValidationErrors { get; set; }

        public Exception Exception;
        public bool Succeded
        {
            get
            {

                return ResultType == ResultType.Success;

            }
            private set { }
        }
        public bool IsSuccessful()
        {
            return this.ResultType == ResultType.Success;
        }
    }
}
