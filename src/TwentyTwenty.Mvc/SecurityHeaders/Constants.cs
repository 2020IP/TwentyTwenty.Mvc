namespace TwentyTwenty.Mvc.SecurityHeaders
{
    /// <summary>
    /// X-Frame-Options-related constants.
    /// </summary>
    public static class FrameOptionsConstants
    {
        /// <summary>
        /// The header value for X-Frame-Options
        /// </summary>
        public const string Header = "X-Frame-Options";

        /// <summary>
        /// The page cannot be displayed in a frame, regardless of the site attempting to do so.
        /// </summary>
        public const string Deny = "DENY";

        /// <summary>
        /// The page can only be displayed in a frame on the same origin as the page itself.
        /// </summary>
        public const string SameOrigin = "SAMEORIGIN";

        /// <summary>
        /// The page can only be displayed in a frame on the specified origin. {0} specifies the format string
        /// </summary>
        public const string AllowFromUri = "ALLOW-FROM {0}";
    }

    /// <summary>
    /// X-XSS-Protection-related constants.
    /// </summary>
    public static class XssProtectionConstants
    {
        /// <summary>
        /// Header value for X-XSS-Protection
        /// </summary>
        public const string Header = "X-XSS-Protection";

        /// <summary>
        /// Enables the XSS Protections
        /// </summary>
        public const string Enabled = "1";

        /// <summary>
        /// Disables the XSS Protections offered by the user-agent.
        /// </summary>
        public const string Disabled = "0";

        /// <summary>
        /// Enables XSS protections and instructs the user-agent to block the response in the event that script has been inserted from user input, instead of sanitizing.
        /// </summary>
        public const string Block = "1; mode=block";

        /// <summary>
        /// A partially supported directive that tells the user-agent to report potential XSS attacks to a single URL. Data will be POST'd to the report URL in JSON format. 
        /// {0} specifies the report url, including protocol
        /// </summary>
        public const string Report = "1; report={0}";
    }

    /// <summary>
    /// X-Content-Type-Options-related constants.
    /// </summary>
    public static class ContentTypeOptionsConstants
    {
        /// <summary>
        /// Header value for X-Content-Type-Options
        /// </summary>
        public const string Header = "X-Content-Type-Options";

        /// <summary>
        /// Disables content sniffing
        /// </summary>
        public const string NoSniff = "nosniff";        
    }

    public static class ReferrerPolicyConstants
    {
        public const string Header = "Referrer-Policy";

        /// <summary>
        /// The Referer header will be omitted entirely. No referrer information is sent along with requests.
        /// </summary>
        public const string NoReferrer = "no-referrer";

        /// <summary>
        /// (default) This is the user agent's default behavior if no policy is specified. The URL is sent as a referrer when the protocol security level stays the same (HTTP→HTTP, HTTPS→HTTPS), but isn't sent to a less secure destination (HTTPS→HTTP).
        /// </summary>
        public const string NoReferrerWhenDowngrade = "no-referrer-when-downgrade";

        /// <summary>
        /// Only send the origin of the document as the referrer in all cases.
        /// The document https://example.com/page.html will send the referrer https://example.com/.
        /// </summary>
        public const string Origin = "origin";

        /// <summary>
        /// Send a full URL when performing a same-origin request, but only send the origin of the document for other cases.
        /// </summary>
        public const string OriginWhenCrossOrigin = "origin-when-cross-origin";

        /// <summary>
        /// A referrer will be sent for same-site origins, but cross-origin requests will contain no referrer information.
        /// </summary>
        public const string SameOrigin = "same-origin";

        /// <summary>
        /// Only send the origin of the document as the referrer when the protocol security level stays the same (HTTPS→HTTPS), but don't send it to a less secure destination (HTTPS→HTTP).
        /// </summary>
        public const string StrictOrigin = "strict-origin";

        /// <summary>
        /// Send a full URL when performing a same-origin request, only send the origin when the protocol security level stays the same (HTTPS→HTTPS), and send no header to a less secure destination (HTTPS→HTTP).
        /// </summary>
        public const string StrictOriginWhenCrossOrigin = "strict-origin-when-cross-origin";

        /// <summary>
        /// Send a full URL when performing a same-origin or cross-origin request.
        /// </summary>
        public const string UnsafeUrl = "unsafe-url";
    }

    /// <summary>
    /// Server headery-related constants.
    /// </summary>
    public static class ServerConstants
    {
        /// <summary>
        /// The header value for X-Powered-By
        /// </summary>
        public static readonly string Header = "Server";
    }
}