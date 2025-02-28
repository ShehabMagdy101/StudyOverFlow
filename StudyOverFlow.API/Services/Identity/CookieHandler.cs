

using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace StudyOverFlow.API.Identity
{
    public class CookieHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
