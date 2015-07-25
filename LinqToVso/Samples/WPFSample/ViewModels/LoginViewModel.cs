using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFSample.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private string _account;
        private ICommand _loginCommand;

        public string Email
        {
            get { return this._email; }
            set { this.Set(() => this.Email, ref this._email, value); }
        }

        public string Password
        {
            get { return this._password; }
            set { this.Set(() => this.Password, ref this._password, value); }
        }

        public string Account
        {
            get { return this._account; }
            set { this.Set(() => this.Account, ref this._account, value); }
        }

        public ICommand LoginCommand
        {
            get
            {
                return this._loginCommand ?? (this._loginCommand = new RelayCommand(
                    async () => { await this.LoginAsyc(); }));
            }
        }

        private async Task LoginAsyc()
        {
            await Task.Delay(0);
            MessageBox.Show("Login...");
        }
    }
}