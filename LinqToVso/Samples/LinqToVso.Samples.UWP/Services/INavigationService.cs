using LinqToVso.Samples.UWP.Helpers;
using LinqToVso.Samples.UWP.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LinqToVso.Samples.UWP.Services
{
    public enum PageKey
    {
        LoginPage,
        MainPage,
        ProjectPage
    }

    public interface INavigationService
    {
        bool CanGoBack { get; }

        void GoBack();

        void NavigateTo(PageKey page);

        void NavigateTo(PageKey page, object parameters);

        void ClearNavigationStack();
    }

    public class NavigationService : INavigationService
    {
        private readonly Dictionary<PageKey, Type> _pages;

        public NavigationService()
        {
            _pages = new Dictionary<PageKey, Type>
            {
                {PageKey.LoginPage, typeof (LoginPage)},
                {PageKey.MainPage, typeof (MainPage)},
                {PageKey.ProjectPage, typeof (ProjectPage)}
            };

            this.CurrentFrame = (Frame)Window.Current.Content;
        }

        private Frame CurrentFrame { get; set; }

        public void GoBack()
        {
            if (CanGoBack)
            {
                CurrentFrame.GoBack();
            }
        }

        public bool CanGoBack => CurrentFrame.CanGoBack;

        public void NavigateTo(PageKey page)
        {
            NavigateTo(page, null);
        }

        public void NavigateTo(PageKey page, object parameters)
        {
            if (!_pages.ContainsKey(page))
            {
                return;
            }

            var pageType = _pages[page];
            NavigateToPage(pageType, parameters);
            TitleBarHelper.ShowBackButton();
        }

        public void ClearNavigationStack()
        {
            try
            {
                CurrentFrame.SetNavigationState("1,0");
                TitleBarHelper.HideBackButton();
            }
            catch (Exception ex)
            {
                // Catch the error
            }
        }

        private void NavigateToPage(Type page, object parameter = null)
        {
            CurrentFrame.Navigate(page, parameter);
        }
    }
}