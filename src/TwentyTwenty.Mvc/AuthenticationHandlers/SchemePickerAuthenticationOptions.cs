using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace TwentyTwenty.Mvc.AuthenticationHandlers
{
    public class SchemePickerAuthenticationOptions : AuthenticationSchemeOptions  
    {
        public Func<HttpContext, string> SchemePicker { get; set; }
        public Func<HttpContext, string, string> ChallengeSchemePicker { get; set; }
    }
}
