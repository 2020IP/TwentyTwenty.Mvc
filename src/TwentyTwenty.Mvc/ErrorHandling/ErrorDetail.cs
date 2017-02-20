namespace TwentyTwenty.Mvc.ErrorHandling
{
    public class ErrorDetails
    {
        public string StackTrace { get; set; }

        public string RequestPath { get; set; }

        public string QueryString { get; set; }
    }
}