using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinqToVso.PCL.Context;

namespace LinqToVso.PCL.Net
{
    public class GetMessageHandler : HttpClientHandler
    {
        private readonly IVsoExecute _exe;
        private readonly IDictionary<string, string> _parameters;
        private readonly string _url;

        public GetMessageHandler(IVsoExecute exe, IDictionary<string, string> parameters, string url)
        {
            this._exe = exe;
            this._parameters = parameters;
            this._url = url;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = this._exe.Authorizer.GetAuthorizationHeaderValue();

            request.Headers.Add("User-Agent", this._exe.UserAgent);
            

            if (this.SupportsAutomaticDecompression)
            {
                this.AutomaticDecompression = DecompressionMethods.GZip;
            }

            this.AllowAutoRedirect = true;
         

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}