using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.Set(() => this.IsBusy, ref this._isBusy, value); }
        }

        public virtual Task OnNavigateTo(object parameter)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigateFrom(object parameter)
        {
            return Task.CompletedTask;
        }
    }
}