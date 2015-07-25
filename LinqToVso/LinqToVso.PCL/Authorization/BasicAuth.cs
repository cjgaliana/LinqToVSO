using System;
using System.Net.Http.Headers;
using System.Text;

namespace LinqToVso.PCL.Authorization
{
    public class BasicAuth : IAuthorizer
    {
        public string User { get; set; }

        public string Password { get; set; }

        public string Account { get; set; }

        public AuthenticationHeaderValue GetAuthorizationHeaderValue()
        {
            return new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.User, this.Password))));
        }
    }
}