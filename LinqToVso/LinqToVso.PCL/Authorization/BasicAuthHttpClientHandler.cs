using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToVso.PCL.Authorization
{
    public class BasicAuthHttpClientHandler : HttpClientHandler
    {
        public BasicAuthHttpClientHandler(string user, string password)
        {
            //this.Credentials = new NetworkCredential(user, password);

            this.User = user;
            this.Password = password;
        }

        public string Password { get; set; }

        public string User { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = this.GetAuthorizationHeaderValue();

            return base.SendAsync(request, cancellationToken);
        }

        private AuthenticationHeaderValue GetAuthorizationHeaderValue()
        {
            return new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.User, this.Password))));
        }
    }
}