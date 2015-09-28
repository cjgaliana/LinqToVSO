using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToVso.Linqify;
using LinqToVso.PCL.Exceptions;

namespace LinqToVso.PCL.Context
{
    public class VsoExecute : ILinqifyExecutor
    {
        internal const string DefaultUserAgent = "Linq-To-VSO/0.1";
        internal const int DefaultReadWriteTimeout = 300000;
        internal const int DefaultTimeout = 100000;

        public VsoExecute()
            : this(new HttpClientHandler())
        {
        }

        public VsoExecute(HttpClientHandler handler)
        {
            this.HttpClientHandler = handler ?? new HttpClientHandler();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public HttpClientHandler HttpClientHandler { get; private set; }

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
        ///     Used in queries to read information from VSO API endpoints.
        /// </summary>
        /// <param name="request">Request with url endpoint and all query parameters</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>XML Respose from VSO</returns>
        public async Task<string> QueryApiAsync<T>(Request request, IRequestProcessor<T> reqProc)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, new Uri(request.FullUrl));

                var parms = request.RequestParameters
                    .ToDictionary(
                        key => key.Name,
                        val => val.Value);

                using (var client = new HttpClient(this.HttpClientHandler))
                {
                    if (this.Timeout != 0)
                    {
                        client.Timeout = TimeSpan.FromSeconds(this.Timeout);
                    }
                    var msg = await client.SendAsync(req, this.CancellationToken).ConfigureAwait(false);

                    return await this.HandleResponseAsync(msg).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                throw;
            }
        }

        public async Task<string> PostToApiAsync<T>(string url, IDictionary<string, string> postData,
            CancellationToken cancelToken)
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

            var content = new StringContent(dataString.ToString().TrimEnd('&'), Encoding.UTF8,
                "application/x-www-form-urlencoded");

            using (var client = new HttpClient(this.HttpClientHandler))
            {
                if (this.Timeout != 0)
                {
                    client.Timeout = TimeSpan.FromSeconds(this.Timeout);
                }

                var msg = await client.PostAsync(url, content, cancelToken).ConfigureAwait(false);

                return await this.HandleResponseAsync(msg);
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
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

            await VsoErrorHandler.ThrowIfErrorAsync(msg).ConfigureAwait(false);

            return await msg.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}