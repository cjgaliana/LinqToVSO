using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using LinqToVso.Linqify;
using LinqToVso.Samples.UWP.Services;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IVsoDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        private IList<Project> _projects = new List<Project>();

        public MainViewModel(INavigationService navigationService, IVsoDataService dataService,
            IDialogService dialogService)
        {
            this._navigationService = navigationService;
            this._dataService = dataService;
            this._dialogService = dialogService;

            this.RefreshCommand = new RelayCommand(async () => await this.RefreshProjectsAsync());
            this.OpenProjectCommand = new RelayCommand<Project>(this.OpenProjectDetails);
        }

        public ICommand RefreshCommand { get; }
        public ICommand OpenProjectCommand { get; private set; }

        public IList<Project> Projects
        {
            get { return this._projects; }
            set { this.Set(() => this.Projects, ref this._projects, value); }
        }

        private void OpenProjectDetails(Project project)
        {
            this._navigationService.NavigateTo(PageKey.ProjectPage, project);
        }

        private async Task RefreshProjectsAsync()
        {
            try
            {
                this.IsBusy = true;
                this.Projects = await this._dataService.Context.Projects.ToListAsync();

                this.IsBusy = false;
            }
            catch (Exception ex)
            {
                this.IsBusy = false;
                await this._dialogService.ShowMessageAsync("Error loading projects", ex.Message);
            }
        }

        public override async Task OnNavigateTo(object parameter)
        {
            await base.OnNavigateTo(parameter);

            if (this.Projects.Any())
            {
                return;
            }

            this.RefreshCommand.Execute(null);
        }
    }
}