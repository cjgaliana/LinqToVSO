using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToVso.Linqify;
using LinqToVso.Samples.UWP.Services;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class ProjectDetailsViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IVsoDataService _vsoDataService;

        private Project _project;

        public ProjectDetailsViewModel(
            INavigationService navigationService,
            IVsoDataService vsoDataService,
            IDialogService dialogService)
        {
            this._navigationService = navigationService;
            this._vsoDataService = vsoDataService;
            this._dialogService = dialogService;
        }

        public Project Project
        {
            get { return this._project; }
            set { this.Set(() => this.Project, ref this._project, value); }
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

        private async Task LoadProjectAsync()
        {
            try
            {
                var teams = await this._vsoDataService.Context.Teams.Where(x => x.ProjectId == this.Project.Id)
                    .Skip(5)
                    .Take(20)
                    .ToListAsync();

                //foreach (var team in teams)
                //{
                //    var teamMembers = await this._vsoDataService
                //                                   .Context
                //                                   .TeamMembers
                //                                   .Where(x => x.ProjectId == this.Project.Id && x.TeamId == team.Id)
                //                                   .ToListAsync();
                //}

                var a = 5;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}