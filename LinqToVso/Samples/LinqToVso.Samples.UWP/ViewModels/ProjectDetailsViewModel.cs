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
    public class ProjectDetailsViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IVsoDataService _vsoDataService;

        private Project _project;
        private IList<Team> _teams;

        public ICommand OpenTeamCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public ProjectDetailsViewModel(
            INavigationService navigationService,
            IVsoDataService vsoDataService,
            IDialogService dialogService)
        {
            this._navigationService = navigationService;
            this._vsoDataService = vsoDataService;
            this._dialogService = dialogService;

            this.CreateCommands();
        }

        public Project Project
        {
            get { return this._project; }
            set { this.Set(() => this.Project, ref this._project, value); }
        }

        public IList<Team> Teams
        {
            get { return this._teams; }
            set { this.Set(() => this.Teams, ref this._teams, value); }
        }

        public override async Task OnNavigateTo(object parameter)
        {
            await base.OnNavigateTo(parameter);

            var project = parameter as Project;
            if (project != null)
            {
                this.Project = project;
                await this.LoadProjectAsync();
            }
            else
            {
                await this._dialogService.ShowMessageAsync("Navigation error", "Project cannot be null");
                this._navigationService.GoBack();
            }
        }

        private void CreateCommands()
        {
            this.OpenTeamCommand = new RelayCommand<Team>((team) => { this._navigationService.NavigateTo(PageKey.TeamPage, team); });
            this.RefreshCommand = new RelayCommand(async () => await this.LoadProjectAsync());
        }

        private async Task LoadProjectAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.LoadTeamsAsync();
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

        private async Task LoadTeamsAsync()
        {
            try
            {
                var teams = await this._vsoDataService.Context.Teams
                    .Where(x => x.ProjectId == this.Project.Id)
                    //.Skip(5)
                    //.Take(20)
                    .ToListAsync();

                this.Teams = teams;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}