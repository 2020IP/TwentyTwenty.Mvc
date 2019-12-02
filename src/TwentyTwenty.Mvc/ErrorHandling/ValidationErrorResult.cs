using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TwentyTwenty.Mvc.ErrorHandling
{
    /// <summary>
    /// Automatically formats validation error response using ModelState
    /// </summary>
    public class ValidationErrorResult : ObjectResult
    {
        private const string DefaultErrorMessage = "Validation failed.";
        private const int DefaultErrorCode = 4000;

        public ValidationErrorResult()
            : this(DefaultErrorCode, DefaultErrorMessage)
        { }

        public ValidationErrorResult(int errorCode)
            : this(errorCode, DefaultErrorMessage)
        { }

        public ValidationErrorResult(string errorMessage)
            : this(DefaultErrorCode, errorMessage)
        { }

        public ValidationErrorResult(int errorCode, string errorMessage)
            : base(null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            Value = new ErrorResponse
            {
                RequestId = context.HttpContext.TraceIdentifier,
                ErrorCode = ErrorCode,
                ErrorMessage = ErrorMessage,
                Errors = context.ModelState.Select(kvp => new ValidationError
                {
                    FieldName = kvp.Key,
                    ErrorMessage = kvp.Value.Errors.FirstOrDefault()?.ErrorMessage,
                })
                .ToArray()
            };

            if (!StatusCode.HasValue)
            {
                StatusCode = 400;
            }

            return base.ExecuteResultAsync(context);
        }
    }
}