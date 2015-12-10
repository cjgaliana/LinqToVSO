using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace LinqToVso.Samples.UWP.Helpers
{
    public class TitleBarHelper
    {
        public static void ShowBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;
        }

        public static void HideBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
        }

        public static void SetTitle(string title)
        {
            ApplicationView.GetForCurrentView().Title = title;
        }

        public static ApplicationViewTitleBar GetTitleBar()
        {
            return ApplicationView.GetForCurrentView().TitleBar;
        }
    }
}