using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;

namespace WPFSample.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {

        private bool _isBusy;

        public bool IsBusy
        {
            get { return this._isBusy; }
            set { this.Set(() => this._isBusy, ref this._isBusy, value); }
        }
    }
}