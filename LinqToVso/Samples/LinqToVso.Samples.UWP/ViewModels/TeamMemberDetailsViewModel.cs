using LinqToVso.Samples.UWP.Services;
using System.Threading.Tasks;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class TeamMemberDetailsViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IVsoDataService _vsoDataService;
        private TeamMember _teamMember;

        public TeamMember TeamMember
        {
            get { return this._teamMember; }
            set { this.Set(() => this.TeamMember, ref this._teamMember, value); }
        }

        public TeamMemberDetailsViewModel(INavigationService navigationService, IDialogService dialogService, IVsoDataService vsoDataService)
        {
            this._navigationService = navigationService;
            this._dialogService = dialogService;
            this._vsoDataService = vsoDataService;
        }

        public override async Task OnNavigateTo(object parameter)
        {
            await base.OnNavigateTo(parameter);

            var teamMember = parameter as TeamMember;
            if (teamMember != null)
            {
                this.TeamMember = teamMember;
            }
            else
            {
                await this._dialogService.ShowMessageAsync("Navigation error", "TeamMember cannot be null");
                this._navigationService.GoBack();
            }
        }
    }
}