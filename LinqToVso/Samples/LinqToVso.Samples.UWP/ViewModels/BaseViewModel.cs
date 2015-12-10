using GalaSoft.MvvmLight;

namespace LinqToVso.Samples.UWP.ViewModels
{
    public class BaseViewModel :ViewModelBase
    {
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { this.Set(() => this.IsBusy, ref this.isBusy, value); }
        }
    }
}