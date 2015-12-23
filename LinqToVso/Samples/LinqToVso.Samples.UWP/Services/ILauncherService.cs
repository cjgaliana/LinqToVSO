using System;
using System.Threading.Tasks;
using Windows.System;

namespace LinqToVso.Samples.UWP.Services
{
    public interface ILauncherService
    {
        Task OpenWebSiteAsync(string url);
    }

    public class LauncherService : ILauncherService
    {
        public async Task OpenWebSiteAsync(string url)
        {
            var success = await Launcher.LaunchUriAsync(new Uri(url));
            if (!success)
            {
                throw new Exception("Is not possible open the requested web site");
            }
        }
    }
}