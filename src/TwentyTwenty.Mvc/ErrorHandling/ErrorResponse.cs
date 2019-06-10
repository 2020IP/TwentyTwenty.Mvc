namespace TwentyTwenty.Mvc.ErrorHandling
{
    public class ErrorResponse
    {
        public string RequestId { get; set; }
        public int ErrorCode { get; set; } = -1;
        public string ErrorMessage { get; set; }
        public bool IsError => ErrorCode > 0;
        public ErrorDetails Details { get; set; }

        public ErrorResponse() {}

        public ErrorResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorCode = 1;
        }

        public ErrorResponse(int errorCode, string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public ErrorResponse(string requestId, string errorMessage)
        {
            ErrorMessage = errorMessage;
            RequestId = requestId;
        }
    }
}