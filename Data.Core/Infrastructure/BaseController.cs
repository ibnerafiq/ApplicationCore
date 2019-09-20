using Data.Core.BaseService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Core.Infrastructure
{
    public class BaseController : ControllerBase
    {
        protected OkObjectResult ReturnJson<T>(Result<T> result)
        {
            try
            {
                if (result != null)
                {
                    if (result.ResultType == ResultType.Exception || result.ResultType == ResultType.Failure || result.ResultType == ResultType.ValidationErrors)
                    {

                        if (result.Message == null || result.Message == string.Empty)
                        {
                            if (result.ResultType == ResultType.ValidationErrors)
                            {
                                result.Message = string.Join(",", result.ValidationErrors.Select(aa => aa.Message));
                            }
                            else
                            {
                                result.Message = "Exception Occurred.";
                            }

                        }

                    }
                }
            }
            catch (Exception)
            { }

            var r = new
            {
                status = result.ResultType.ToString(),
                text = result.Message ?? "Task has been done, successfully",
                @object = result.Data
            };

            return Ok(r);
        }
        protected OkObjectResult ReturnJson<T>(IdentityResult<T> result)
        {


            var r = new
            {
                status = result.Succeeded ? "Success" : "Error",
                text = result.Succeeded ? "Task has been done, successfully" : result.Errors == null || result.Errors.Count() == 0 ? "Exception Occured." : string.Join(",", result.Errors),
                @object = result.Data
            };

            return Ok(r);
        }
        protected OkObjectResult ReturnJson(IdentityResult result)
        {


            var r = new
            {
                status = result.Succeeded ? "Success" : "Error",
                text = result.Succeeded ? "Task has been done, successfully" : result.Errors == null || result.Errors.Count() == 0 ? "Exception Occured." : string.Join(",", result.Errors),
                @object = string.Empty
            };

            return Ok(r);
        }
    }
}
