using System;

namespace TwentyTwenty.Mvc.SecurityHeaders
{
    public class SecurityHeadersBuilder
    {
        private readonly SecurityHeadersPolicy _policy = new();

        public SecurityHeadersBuilder AddDefaultSecurePolicy()
        {
            AddFrameOptionsDeny();
            AddXssProtectionBlock();
            AddContentTypeOptionsNoSniff();
            RemoveServerHeader();
            return this;
        }

        /// <summary>
        /// Add X-Frame-Options DENY to all requests.
        /// The page cannot be displayed in a frame, regardless of the site attempting to do so
        /// </summary>
        public SecurityHeadersBuilder AddFrameOptionsDeny()
        {
            _policy.SetHeaders[FrameOptionsConstants.Header] = FrameOptionsConstants.Deny;
            return this;
        }

        /// <summary>
        /// Add X-Frame-Options SAMEORIGIN to all requests.
        /// The page can only be displayed in a frame on the same origin as the page itself.
        /// </summary>
        public SecurityHeadersBuilder AddFrameOptionsSameOrigin()
        {
            _policy.SetHeaders[FrameOptionsConstants.Header] = FrameOptionsConstants.SameOrigin;
            return this;
        }

        /// <summary>
        /// Add X-Frame-Options ALLOW-FROM {uri} to all requests, where the uri is provided
        /// The page can only be displayed in a frame on the specified origin.
        /// </summary>
        /// <param name="uri">The uri of the origin in which the page may be displayed in a frame</param>
        public SecurityHeadersBuilder AddFrameOptionsAllowFrom(string uri)
        {
            _policy.SetHeaders[FrameOptionsConstants.Header] = string.Format(FrameOptionsConstants.AllowFromUri, uri);
            return this;
        }

        /// <summary>
        /// Add X-XSS-Protection 1 to all requests.
        /// Enables the XSS Protections
        /// </summary>
        public SecurityHeadersBuilder AddXssProtectionEnabled()
        {
            _policy.SetHeaders[XssProtectionConstants.Header] = XssProtectionConstants.Enabled;
            return this;
        }

        /// <summary>
        /// Add X-XSS-Protection 0 to all requests.
        /// Disables the XSS Protections offered by the user-agent.
        /// </summary>
        public SecurityHeadersBuilder AddXssProtectionDisabled()
        {
            _policy.SetHeaders[XssProtectionConstants.Header] = XssProtectionConstants.Disabled;
            return this;
        }

        /// <summary>
        /// Add X-XSS-Protection 1; mode=block to all requests.
        /// Enables XSS protections and instructs the user-agent to block the response in the event that script has been inserted from user input, instead of sanitizing.
        /// </summary>
        public SecurityHeadersBuilder AddXssProtectionBlock()
        {
            _policy.SetHeaders[XssProtectionConstants.Header] = XssProtectionConstants.Block;
            return this;
        }

        /// <summary>
        /// Add X-XSS-Protection 1; report=http://site.com/report to all requests.
        /// A partially supported directive that tells the user-agent to report potential XSS attacks to a single URL. Data will be POST'd to the report URL in JSON format.
        /// </summary>
        public SecurityHeadersBuilder AddXssProtectionReport(string reportUrl)
        {
            _policy.SetHeaders[XssProtectionConstants.Header] =
                string.Format(XssProtectionConstants.Report, reportUrl);
            return this;
        }

        /// <summary>
        /// Add X-Content-Type-Options nosniff to all requests.
        /// Can be set to protect against MIME type confusion attacks.
        /// </summary>
        public SecurityHeadersBuilder AddContentTypeOptionsNoSniff()
        {
            _policy.SetHeaders[ContentTypeOptionsConstants.Header] = ContentTypeOptionsConstants.NoSniff;
            return this;
        }

        /// <summary>
        /// The Referer header will be omitted entirely. No referrer information is sent along with requests.
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicyNoReferrer()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.NoReferrer;
            return this;
        }

        /// <summary>
        /// (default) This is the user agent's default behavior if no policy is specified. The URL is sent as a referrer when the protocol security level stays the same (HTTP→HTTP, HTTPS→HTTPS), but isn't sent to a less secure destination (HTTPS→HTTP).
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicyNoReferrerWhenDowngrade()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.NoReferrerWhenDowngrade;
            return this;
        }

        /// <summary>
        /// Only send the origin of the document as the referrer in all cases.
        /// The document https://example.com/page.html will send the referrer https://example.com/.
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicyOrigin()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.Origin;
            return this;
        }

        /// <summary>
        /// Send a full URL when performing a same-origin request, but only send the origin of the document for other cases.
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicyOriginWhenCrossOrigin()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.OriginWhenCrossOrigin;
            return this;
        }

        /// <summary>
        /// A referrer will be sent for same-site origins, but cross-origin requests will contain no referrer information.
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicySameOrigin()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.SameOrigin;
            return this;
        }

        /// <summary>
        /// Only send the origin of the document as the referrer when the protocol security level stays the same (HTTPS→HTTPS), but don't send it to a less secure destination (HTTPS→HTTP).
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicyStrictOrigin()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.StrictOrigin;
            return this;
        }

        /// <summary>
        /// Send a full URL when performing a same-origin request, only send the origin when the protocol security level stays the same (HTTPS→HTTPS), and send no header to a less secure destination (HTTPS→HTTP).
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicyStrictOriginWhenCrossOrigin()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.StrictOriginWhenCrossOrigin;
            return this;
        }

        /// <summary>
        /// Send a full URL when performing a same-origin or cross-origin request.
        /// </summary>
        public SecurityHeadersBuilder AddReferrerPolicyUnsafeUrl()
        {
            _policy.SetHeaders[ReferrerPolicyConstants.Header] = ReferrerPolicyConstants.UnsafeUrl;
            return this;
        }

        /// <summary>
        /// Removes the Server header from all responses
        /// </summary>
        public SecurityHeadersBuilder RemoveServerHeader()
        {
            _policy.RemoveHeaders.Add(ServerConstants.Header);
            return this;
        }

        /// <summary>
        /// Adds a custom header to all requests
        /// </summary>
        /// <param name="header">The header name</param>
        /// <param name="value">The value for the header</param>
        /// <returns></returns>
        public SecurityHeadersBuilder AddCustomHeader(string header, string value)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            _policy.SetHeaders[header] = value;
            return this;
        }

        /// <summary>
        /// Remove a header from all requests
        /// </summary>
        /// <param name="header">The to remove</param>
        /// <returns></returns>
        public SecurityHeadersBuilder RemoveHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            _policy.RemoveHeaders.Add(header);
            return this;
        }

        /// <summary>
        /// Builds a new <see cref="SecurityHeadersPolicy"/> using the entries added.
        /// </summary>
        /// <returns>The constructed <see cref="SecurityHeadersPolicy"/>.</returns>
        public SecurityHeadersPolicy Build()
        {
            return _policy;
        }
    }
}