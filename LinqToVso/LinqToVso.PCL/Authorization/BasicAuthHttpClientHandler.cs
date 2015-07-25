using System.Net.Http;

namespace LinqToVso.PCL.Authorization
{
    public class BasicAuthHttpClientHandler : HttpClientHandler
    {
        public BasicAuthHttpClientHandler(string user, string password)
        {
            this.Credentials = new System.Net.NetworkCredential(user, password);
        }
    }
}