using GalaSoft.MvvmLight.Command;
using LinqToVso.Linqify;
using LinqToVso.Samples.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class TeamViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IVsoDataService _vsoDataService;
        private Team _team;
        private IList<TeamMember> _teamMembers;

        public TeamViewModel(INavigationService navigationService, IDialogService dialogService,
            IVsoDataService vsoDataService)
        {
            this._navigationService = navigationService;
            this._dialogService = dialogService;
            this._vsoDataService = vsoDataService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            this.OpenTeamMemberCommand = new RelayCommand<TeamMember>((teamMember) => this._navigationService.NavigateTo(PageKey.TeamMemberPage, teamMember));
        }

        public ICommand OpenTeamMemberCommand { get; private set; }

        public Team Team
        {
            get { return this._team; }
            set { this.Set(() => this.Team, ref this._team, value); }
        }

        public IList<TeamMember> TeamMembers
        {
            get { return this._teamMembers; }
            set { this.Set(() => this.TeamMembers, ref this._teamMembers, value); }
        }

        public override async Task OnNavigateTo(object parameter)
        {
            await base.OnNavigateTo(parameter);

            var team = parameter as Team;
            if (team != null)
            {
                this.Team = team;
                await this.LoadUsersAsync();
            }
            else
            {
                await this._dialogService.ShowMessageAsync("Navigation error", "Team cannot be null");
                this._navigationService.GoBack();
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                this.IsBusy = true;

                var members = await this._vsoDataService.Context.TeamMembers
                    .Where(x => x.ProjectId == this.Team.ProjectId && x.TeamId == this.Team.Id)
                    .ToListAsync();

                this.TeamMembers = members;
            }
            catch (Exception ex)
            {
                await this._dialogService.ShowMessageAsync("Error", ex.Message);
                throw;
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}