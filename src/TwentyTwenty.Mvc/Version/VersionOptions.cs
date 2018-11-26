namespace TwentyTwenty.Mvc.Version
{
    public class VersionOptions
    {
        private const string DefaultHeader = "x-api-version";

        /// <summary>
        /// The header field name where the correlation ID will be stored
        /// </summary>
        public string Header { get; set; } = DefaultHeader;
    }
}