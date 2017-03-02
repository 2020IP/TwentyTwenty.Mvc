namespace TwentyTwenty.Mvc
{
    /// <summary>
    /// Maps error code numbers to HTTP status codes
    /// </summary>
    public interface ICodeMap
    {
        /// <summary>
        /// Maps an error code to an HTTP status code
        /// </summary>
        /// <param name="errorCode">The TwentyTwentyExceptino error code</param>
        /// <returns>The HTTP status code.</returns>
        int MapErrorCode(int errorCode);
    }
}