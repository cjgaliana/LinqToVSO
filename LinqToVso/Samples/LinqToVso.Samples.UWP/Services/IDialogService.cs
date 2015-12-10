using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace LinqToVso.Samples.UWP.Services
{
    public interface IDialogService
    {
        Task ShowMessageAsync(string caption, string message);
    }

    public class DialogService : IDialogService
    {
        public async Task ShowMessageAsync(string caption, string message)
        {
            var dialog = new MessageDialog(message, caption);
            await dialog.ShowAsync();
        }
    }
}