using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.BaseService
{
    public abstract class BaseService
    {
        #region Validation
        public List<ValidationResult> Errors { get; set; }
        public bool Validate(object model)
        {
            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
            this.Errors = validationResults;
            return this.Errors.Count == 0;
        }
        #endregion

        #region Returns

        public Result<T> ReturnException<T>(Exception error)
        {
            string message = "Service Error";
            if (error != null)
            {
                if (error.InnerException != null && error.InnerException.InnerException != null)//.IsNotNullOrEmpty())
                {
                    message = error.InnerException.InnerException.Message;
                }
                else
                {
                    if (error.Message != null) //    .IsNotNullOrEmpty())
                    {
                        message = error.Message;
                    }
                }



            }

            return new Result<T>
            {
                Exception = error,
                Message = message,
                ResultType = ResultType.Exception
            };
        }

        public Result<T> ReturnSuccess<T>(T data)
        {
            return new Result<T>
            {
                ResultType = ResultType.Success,
                Data = data,
                ValidationErrors = null
            };
        }

        public Result<T> ReturnSuccess<T>(string message)
        {
            return new Result<T>
            {
                ResultType = ResultType.Success,
                Data = default(T),
                Message = message,
                ValidationErrors = null
            };
        }

        public Result<T> ReturnSuccess<T>(string message, T model)
        {
            return new Result<T>
            {
                ResultType = ResultType.Success,
                Data = model,
                Message = message,
                ValidationErrors = null
            };
        }

        public Result<T> ReturnErrorMessage<T>(string errorMessage)
        {
            return new Result<T>
            {
                ResultType = ResultType.Failure,
                Message = errorMessage
            };
        }

        public Result<T> ReturnErrorMessage<T>(string errorMessage, T data)
        {
            return new Result<T>
            {
                ResultType = ResultType.Failure,
                Message = errorMessage,
                Data = data
            };
        }

        public Result<T> ReturnEmpty<T>(T data)
        {
            return new Result<T>
            {
                ResultType = ResultType.Empty,
                Data = default(T),
                ValidationErrors = null
            };
        }

        public Result<T> ReturnFailure<T>(T data)
        {
            return new Result<T>
            {
                ResultType = ResultType.Failure,
                Data = default(T),
                ValidationErrors = null
            };
        }
        public Result<T> ReturnValidationErrors<T>()
        {
            return new Result<T>
            {
                ResultType = ResultType.ValidationErrors,
                ValidationErrors = this.Errors.ToValidationErrors()
            };
        }
        public Result<T> ReturnValidationErrors<T>(List<ValidationResult> errors)
        {
            return new Result<T>
            {
                ResultType = ResultType.ValidationErrors,
                ValidationErrors = errors.ToValidationErrors()
            };
        }

        public Result<T> ReturnValidationErrors<T>(params ValidationResult[] errors)
        {
            return new Result<T>
            {
                ResultType = ResultType.ValidationErrors,
                ValidationErrors = errors.ToList().ToValidationErrors()
            };
        }
        public Result<T> ReturnValidationErrors<T>(params ValidationError[] errors)
        {
            return new Result<T>
            {
                ResultType = ResultType.ValidationErrors,
                ValidationErrors = errors.ToList()
            };
        }
        #endregion



    }
}
