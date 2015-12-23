using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToVso
{
    public class BasicAuthHttpClientHandler : HttpClientHandler
    {
        private readonly string _password;
        private readonly string _user;

        public BasicAuthHttpClientHandler(string user, string password)
        {
            this._user = user;
            this._password = password;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = this.GetAuthorizationHeaderValue();

            return base.SendAsync(request, cancellationToken);
        }

        private AuthenticationHeaderValue GetAuthorizationHeaderValue()
        {
            return new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this._user, this._password))));
        }
    }
}