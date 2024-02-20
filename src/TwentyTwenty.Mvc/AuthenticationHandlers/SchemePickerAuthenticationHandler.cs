using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TwentyTwenty.Mvc.AuthenticationHandlers
{
    public class SchemePickerAuthenticationHandler : AuthenticationHandler<SchemePickerAuthenticationOptions>  
    {
        public SchemePickerAuthenticationHandler(
            IOptionsMonitor<SchemePickerAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder) : base(options, logger, encoder) { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var scheme = Options.SchemePicker.Invoke(Context);
            return await Context.AuthenticateAsync(scheme);;
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var scheme = Options.SchemePicker.Invoke(Context);
            var challengeScheme = Options.ChallengeSchemePicker.Invoke(Context, scheme);
            await Context.ChallengeAsync(challengeScheme, properties);
        }
    }
}
