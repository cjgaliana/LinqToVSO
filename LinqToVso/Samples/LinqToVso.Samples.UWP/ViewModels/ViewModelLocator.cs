using GalaSoft.MvvmLight.Ioc;
using LinqToVso.Samples.UWP.Services;
using Microsoft.Practices.ServiceLocation;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();
            RegisterViewModels();
        }

        public LoginViewModel LoginViewModel => SimpleIoc.Default.GetInstance<LoginViewModel>();
        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();

        private void RegisterServices()
        {
            SimpleIoc.Default.Register<IVsoDataService, VsoDataService>();
            SimpleIoc.Default.Register<INetworkService, NetworkService>();
            SimpleIoc.Default.Register<IAuthenticationService, BasicAuthenticationService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<ILauncherService, LauncherService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
        }

        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
        }
    }
}