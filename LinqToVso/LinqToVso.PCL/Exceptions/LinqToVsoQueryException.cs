using System;
using System.Net;

namespace LinqToVso.Exceptions
{
    public class LinqToVsoQueryException : Exception
    {
        public LinqToVsoQueryException()
            : this(string.Empty)
        {
        }

        public LinqToVsoQueryException(string message)
            : base(message)
        {
        }

        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public int ErrorCode { get; set; }
    }
}