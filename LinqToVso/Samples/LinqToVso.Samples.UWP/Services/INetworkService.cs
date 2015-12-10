using Windows.Networking.Connectivity;

namespace LinqToVso.Samples.UWP.Services
{
    public interface INetworkService
    {
        bool IsOnline { get; }
    }

    public class DesignNetworkService : INetworkService
    {
        public bool IsOnline => true;
    }

    public class NetworkService : INetworkService
    {
        public bool IsOnline
        {
            get
            {
                var internetProfile = NetworkInformation.GetInternetConnectionProfile();
                return internetProfile != null
                       && internetProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }
        }
    }
}