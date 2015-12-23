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
            this._navigationService = navigationService;
            this._authenticationService = authenticationService;
            this._launcherService = launcherService;
            this._dialogService = dialogService;
            this._vsoDataService = vsoDataService;

            this.LoginCommand = new RelayCommand(async () => await this.LoginAsync());
            this.HelpCommand = new RelayCommand(async () => await this.OpenHelpAsync());
        }

        public ICommand LoginCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }

        public string Password
        {
            get { return this._password; }
            set { this.Set(() => this.Password, ref this._password, value); }
        }

        public string Username
        {
            get { return this._username; }
            set { this.Set(() => this.Username, ref this._username, value); }
        }

        public string Account
        {
            get { return this._account; }
            set { this.Set(() => this.Account, ref this._account, value); }
        }

        private async Task OpenHelpAsync()
        {
            var url = "https://www.visualstudio.com/integrate/get-started/auth/overview";
            await this._launcherService.OpenWebSiteAsync(url);
        }

        private async Task LoginAsync()
        {
            try
            {
                this.IsBusy = true;
                await this._authenticationService.LoginAsync(this.Account, this.Username, this.Password);
                this._vsoDataService.Initialize(this.Account, this.Username, this.Password);
                this.IsBusy = false;
                this._navigationService.NavigateTo(PageKey.MainPage);
                this._navigationService.ClearNavigationStack();
            }
            catch (Exception ex)
            {
                this.IsBusy = false;
                await this._dialogService.ShowMessageAsync("Error login", ex.Message);
            }
        }
    }
}