using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using LinqToVso.Samples.UWP.Services;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;
        private readonly ILauncherService _launcherService;
        private readonly INavigationService _navigationService;
        private readonly IVsoDataService _vsoDataService;

        private string _account;
        private string _password;
        private string _username;

        public LoginViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            ILauncherService launcherService,
            IDialogService dialogService,
            IVsoDataService vsoDataService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            _launcherService = launcherService;
            _dialogService = dialogService;
            _vsoDataService = vsoDataService;

            LoginCommand = new RelayCommand(async () => await LoginAsync());
            HelpCommand = new RelayCommand(async () => await OpenHelpAsync());
        }

        public ICommand LoginCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }

        public string Password
        {
            get { return _password; }
            set { Set(() => Password, ref _password, value); }
        }

        public string Username
        {
            get { return _username; }
            set { Set(() => Username, ref _username, value); }
        }

        public string Account
        {
            get { return _account; }
            set { Set(() => Account, ref _account, value); }
        }

        private async Task OpenHelpAsync()
        {
            var url = "https://www.visualstudio.com/integrate/get-started/auth/overview";
            await _launcherService.OpenWebSiteAsync(url);
        }

        private async Task LoginAsync()
        {
            try
            {
                IsBusy = true;
                await _authenticationService.LoginAsync(Account, Username, Password);
                _vsoDataService.Initialize(Account, Username, Password);
                IsBusy = false;
                _navigationService.NavigateTo(PageKey.MainPage);
                _navigationService.ClearNavigationStack();
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await _dialogService.ShowMessageAsync("Error login", ex.Message);
            }
        }
    }
}