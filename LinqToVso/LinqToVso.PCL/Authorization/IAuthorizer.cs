using System.Net.Http.Headers;

namespace LinqToVso.PCL.Authorization
{
    public interface IAuthorizer
    {
        string Account { get; set; }

        AuthenticationHeaderValue GetAuthorizationHeaderValue();
    }
}