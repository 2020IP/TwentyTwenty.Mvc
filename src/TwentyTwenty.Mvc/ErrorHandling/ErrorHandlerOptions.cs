using System;

namespace TwentyTwenty.Mvc.ErrorHandling
{
    public class ErrorHandlerOptions
    {
        /// <summary>
        /// Gets or sets an option controlling whether to use the HTML error page in response to non-ajax requests. Defaults to TRUE.
        /// </summary>
        public bool UseHtmlPage { get; set; } = true;

        /// <summary>
        /// Gets or sets the codemap. Codemap allows mapping TwentyTwentyException error codes to http status codes.
        /// </summary>
        public Func<int, int> CodeMap { get; set; }
    }
}