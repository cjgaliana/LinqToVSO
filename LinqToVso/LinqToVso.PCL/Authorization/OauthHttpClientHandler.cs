using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToVso.PCL.Authorization
{
    public class OauthHttpClientHandler : HttpClientHandler
    {
        private readonly string _token;

        public OauthHttpClientHandler(string token)
        {
            this._token = token;
        }

        public string Password { get; set; }
        public string User { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = this.GetAuthorizationHeaderValue();

            return base.SendAsync(request, cancellationToken);
        }

        private AuthenticationHeaderValue GetAuthorizationHeaderValue()
        {
            return new AuthenticationHeaderValue("Bearer", this._token);
        }
    }
}