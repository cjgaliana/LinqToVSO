using System;
using System.Collections.Generic;
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

        private IList<Project> _projects;

        public MainViewModel(INavigationService navigationService, IVsoDataService dataService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
            _dialogService = dialogService;

            RefreshCommand = new RelayCommand(async () => await RefreshProjectsAsync());
        }

        public ICommand RefreshCommand { get; private set; }

        public IList<Project> Projects
        {
            get { return _projects; }
            set { Set(() => Projects, ref _projects, value); }
        }

        private async Task RefreshProjectsAsync()
        {
            try
            {
                IsBusy = true;
                Projects = await _dataService.Context.Projects.ToListAsync();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await _dialogService.ShowMessageAsync("Error loading projects", ex.Message);
            }
        }
    }
}