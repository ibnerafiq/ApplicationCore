using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Core.BaseService
{
    public class IdentityResult<T> : IdentityResult
    {
        public T Data { get; set; }

        public IdentityResult(bool success)
         
        {
            this.Succeeded = success;
        }

        public IdentityResult(params string[] errors)
          //  : base(errors)
        {
            errors.ToList().ForEach(aa =>
            {
                
                this.Errors.ToList().Add(new IdentityError { Code = aa, Description = aa });
            });
            
        }
        public IdentityResult(IEnumerable<string> errors)
            //: base(errors)
        {
            errors.ToList().ForEach(aa =>
            {

                this.Errors.ToList().Add(new IdentityError { Code = aa, Description = aa });
            });
        }

        public void AddErrors(ModelStateDictionary ModelState)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(aa => aa.ErrorMessage).ToList();
            errors.ForEach(aa =>
            {
                this.Errors.ToList().Add(new IdentityError { Code = aa, Description = aa });
            });
            
        }
    }
}
