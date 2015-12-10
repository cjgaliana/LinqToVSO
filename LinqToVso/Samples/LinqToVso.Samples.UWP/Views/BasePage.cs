using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LinqToVso.Samples.UWP.ViewModels;

namespace LinqToVso.Samples.UWP.Views
{
    public class BasePage : Page
    {
        private BaseViewModel BaePageViewModel => this.DataContext as BaseViewModel;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.BaePageViewModel.OnNavigateFrom(e.Parameter);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.BaePageViewModel.OnNavigateTo(e.Parameter);
        }
    }
}