using System.Net.Http.Headers;
using LinqToVso.PCL.Authorization;
using LinqToVso.PCL.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToVso.PCL.Context
{
    public class VsoExecute : IVsoExecute, IDisposable
    {
        internal const string DefaultUserAgent = "Linq-To-VSO/0.1";
        internal const int DefaultReadWriteTimeout = 300000;
        internal const int DefaultTimeout = 100000;
        private readonly IAuthorizer _authorizer;

        public VsoExecute(IAuthorizer authorizer)
        {
            if (authorizer == null)
            {
                throw new ArgumentNullException("authorizer", "Authorizer cannot be null");
            }

            this._authorizer = authorizer;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IAuthorizer Authorizer
        {
            get { return this._authorizer; }
        }

        /// <summary>
        ///     Allows callers to cancel operation (where applicable)
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        ///     Timeout (milliseconds) to wait for a server response
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        ///     Gets the most recent URL executed
        /// </summary>
        /// <remarks>
        ///     This is very useful for debugging
        /// </remarks>
        public Uri LastUrl { get; private set; }

        /// <summary>
        ///     list of response headers from query
        /// </summary>
        public IDictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        ///     Gets and sets HTTP UserAgent header
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     Timeout (milliseconds) for writing to request
        ///     stream or reading from response stream
        /// </summary>
        public int ReadWriteTimeout { get; set; }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    StreamingCallbackAsync = null;

            //    if (Log != null)
            //    {
            //        Log.Dispose();
            //    }
            //}
        }

        /// <summary>
        ///     Used in queries to read information from VSO API endpoints.
        /// </summary>
        /// <param name="request">Request with url endpoint and all query parameters</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>XML Respose from VSO</returns>
        public async Task<string> QueryVsoAsync<T>(Request request, IRequestProcessor<T> reqProc)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, new Uri(request.FullUrl));

                Dictionary<string, string> parms = request.RequestParameters
                    .ToDictionary(
                        key => key.Name,
                        val => val.Value);

                //var handler = new GetMessageHandler(this, parms, request.FullUrl);
                var handler = new HttpClientHandler { AllowAutoRedirect = true };

                //using (var client = new HttpClient(handler))
                using (var client = new HttpClient(handler))
                {
                    if (this.Timeout != 0)
                    {
                        client.Timeout = TimeSpan.FromSeconds(this.Timeout);
                    }

                    client.DefaultRequestHeaders.Authorization = this.Authorizer.GetAuthorizationHeaderValue();
                    //HttpResponseMessage msg = await client.SendAsync(req, this.CancellationToken).ConfigureAwait(false);

                    //return await this.HandleResponseAsync(msg).ConfigureAwait(false);
                    var msg = await client.GetStringAsync(request.FullUrl).ConfigureAwait(false);
                    return msg;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                throw;
            }
        }

        public async Task<string> PostToVsoAsync<T>(string url, IDictionary<string, string> postData, CancellationToken cancelToken)
        {
            var cleanPostData = new Dictionary<string, string>();

            var dataString = new StringBuilder();

            foreach (var pair in postData)
            {
                if (pair.Value != null)
                {
                    dataString.AppendFormat("{0}={1}&", pair.Key, Url.PercentEncode(pair.Value));
                    cleanPostData.Add(pair.Key, pair.Value);
                }
            }

            var content = new StringContent(dataString.ToString().TrimEnd('&'), Encoding.UTF8, "application/x-www-form-urlencoded");
            var handler = new PostMessageHandler(this, cleanPostData, url);
            using (var client = new HttpClient(handler))
            {
                if (Timeout != 0)
                {
                    client.Timeout = TimeSpan.FromSeconds(Timeout);
                }

                HttpResponseMessage msg = await client.PostAsync(url, content, cancelToken).ConfigureAwait(false);

                return await HandleResponseAsync(msg);
            }
        }

        private async Task<string> HandleResponseAsync(HttpResponseMessage msg)
        {
            this.LastUrl = msg.RequestMessage.RequestUri;

            this.ResponseHeaders =
                (from header in msg.Headers
                 select new
                 {
                     header.Key,
                     Value = string.Join(", ", header.Value)
                 })
                    .ToDictionary(
                        pair => pair.Key,
                        pair => pair.Value);

            //await TwitterErrorHandler.ThrowIfErrorAsync(msg).ConfigureAwait(false);

            return await msg.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}