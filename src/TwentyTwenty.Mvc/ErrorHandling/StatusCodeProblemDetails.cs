using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace TwentyTwenty.Mvc.ErrorHandling
{
    /// <summary>
    /// A basic problem details representation for an HTTP status code.
    /// It includes default values for <see cref="MvcProblemDetails.Type"/> and <see cref="MvcProblemDetails.Title"/>.
    /// </summary>
    public class StatusCodeProblemDetails : ProblemDetails
    {
        public StatusCodeProblemDetails(int statusCode)
        {
            SetDetails(this, statusCode);
        }

        public static ProblemDetails Create(int statusCode)
        {
            var details = new ProblemDetails();

            SetDetails(details, statusCode);

            return details;
        }

        internal static ProblemDetails Create(int statusCode, string title)
        {
            var details = Create(statusCode);

            details.Title = title;

            return details;
        }

        private static void SetDetails(ProblemDetails details, int statusCode)
        {
            details.Status = statusCode;
            details.Type = GetDefaultType(statusCode);
            details.Title = ReasonPhrases.GetReasonPhrase(statusCode);
        }

        internal static string GetDefaultType(int statusCode)
        {
            return $"https://httpstatuses.com/{statusCode}";
        }
    }
}
