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
        private IList<Hook> _consumerHooks;
        private IList<Process> _processes;

        private IList<Project> _projects = new List<Project>();
        private IList<Hook> _publisherHooks;
        private IList<Subscription> _subscriptions;

        public MainViewModel(INavigationService navigationService, IVsoDataService dataService,
            IDialogService dialogService)
        {
            this._navigationService = navigationService;
            this._dataService = dataService;
            this._dialogService = dialogService;

            this.RefreshCommand = new RelayCommand(async () => await this.RefreshProjectsAsync());
            this.OpenProjectCommand = new RelayCommand<Project>(this.OpenProjectDetails);
            this.OpenProcessCommand = new RelayCommand<Process>(this.OpenProcessDetails);
            this.OpenHookCommand = new RelayCommand<Hook>(this.OpenHookDetails);
            this.OpenSubscriptionCommand = new RelayCommand<Subscription>(subscription =>
            {
                /*TODO*/
            });
        }

        public ICommand RefreshCommand { get; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand OpenProcessCommand { get; private set; }
        public ICommand OpenHookCommand { get; private set; }
        public ICommand OpenSubscriptionCommand { get; private set; }

        public IList<Project> Projects
        {
            get { return this._projects; }
            set { this.Set(() => this.Projects, ref this._projects, value); }
        }

        public IList<Process> Processes
        {
            get { return this._processes; }
            set { this.Set(() => this.Processes, ref this._processes, value); }
        }

        public IList<Hook> ConsumerHooks
        {
            get { return this._consumerHooks; }
            set { this.Set(() => this.ConsumerHooks, ref this._consumerHooks, value); }
        }

        public IList<Hook> PublisherHooks
        {
            get { return this._publisherHooks; }
            set { this.Set(() => this.PublisherHooks, ref this._publisherHooks, value); }
        }

        public IList<Subscription> Subscriptions
        {
            get { return this._subscriptions; }
            set { this.Set(() => this.Subscriptions, ref this._subscriptions, value); }
        }

        private void OpenProjectDetails(Project project)
        {
            this._navigationService.NavigateTo(PageKey.ProjectPage, project);
        }

        private void OpenProcessDetails(Process process)
        {
            this._navigationService.NavigateTo(PageKey.ProcessPage, process);
        }

        private void OpenHookDetails(Hook hook)
        {
            this._navigationService.NavigateTo(PageKey.HookPage, hook);
        }

        private async Task RefreshProjectsAsync()
        {
            try
            {
                this.IsBusy = true;
                this.Projects = await this._dataService.Context.Projects.ToListAsync();
                this.Processes = await this._dataService.Context.Processes.ToListAsync();
                this.ConsumerHooks =
                    await this._dataService.Context.Hooks.Where(x => x.Type == HookType.Consumer).ToListAsync();
                this.PublisherHooks =
                    await this._dataService.Context.Hooks.Where(x => x.Type == HookType.Publisher).ToListAsync();
                this.Subscriptions = await this._dataService.Context.Subscriptions.ToListAsync();

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