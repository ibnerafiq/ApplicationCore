using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Data.Core.BaseService
{
    public static class ValidationResultEx
    {

        public static ValidationError ToValidationError(
            this ValidationResult result)
        {
            return new ValidationError
            {
                Message = result.ErrorMessage,
                Field = result.MemberNames.FirstOrDefault()
            };
        }

        public static List<ValidationError> ToValidationErrors(
            this List<ValidationResult> results)
        {
            var list = new List<ValidationError>();
            for (int i = 0; i < results.Count; i++)
            {
                list.Add(new ValidationError
                {
                    Message = results[i].ErrorMessage,
                    Field = results[i].MemberNames.FirstOrDefault()
                });
            }

            return list;
        }
    }
}
