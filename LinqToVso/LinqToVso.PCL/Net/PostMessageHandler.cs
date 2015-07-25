using LinqToVso.PCL.Context;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToVso.PCL.Net
{
    public class PostMessageHandler : HttpClientHandler
    {
        private readonly IVsoExecute _exe;
        private readonly IDictionary<string, string> _postData;
        private readonly string _url;

        public PostMessageHandler(IVsoExecute exe, IDictionary<string, string> postData, string url)
        {
            this._exe = exe;
            this._postData = postData;
            this._url = url;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = this._exe.Authorizer.GetAuthorizationHeaderValue();

            request.Headers.Add("User-Agent", this._exe.UserAgent);
            request.Headers.ExpectContinue = false;
            request.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            if (this.SupportsAutomaticDecompression)
            {
                this.AutomaticDecompression = DecompressionMethods.GZip;
            }

       

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}