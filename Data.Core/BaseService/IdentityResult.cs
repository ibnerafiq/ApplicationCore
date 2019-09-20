using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.BaseService
{
    blic class IdentityResult<T> : IdentityResult
    {
        public T Data { get; set; }

        public IdentityResult(bool success)
            : base(success)
        {

        }

        public IdentityResult(params string[] errors)
            : base(errors)
        {

        }
        public IdentityResult(IEnumerable<string> errors)
            : base(errors)
        {

        }

        public void AddErrors(ModelStateDictionary ModelState)
        {
            Errors.ToList().AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(xx => xx.ErrorMessage).ToList());
        }
    }
}
