namespace TwentyTwenty.Mvc.ErrorHandling
{
    public class ErrorResponse
    {
        public string RequestId { get; set; }

        public int ErrorCode { get; set; } = -1;

        public string ErrorMessage { get; set; }

        public bool IsError => ErrorCode > 0;

        public ErrorDetails Details { get; set; }
    }
}