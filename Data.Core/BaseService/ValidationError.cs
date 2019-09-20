using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.BaseService
{
    public class ValidationError
    {
        // Summary:
        //     Gets the error message for the validation.
        //
        // Returns:
        //     The error message for the validation.
        public string Message { get; set; }
        //
        // Summary:
        //     Gets the collection of member names that indicate which fields have validation
        //     errors.
        //
        // Returns:
        //     The collection of member names that indicate which fields have validation
        //     errors.
        public string Field { get; set; }

        public string FriendlyField { get; set; }
    }
}
