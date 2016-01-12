using LinqToVso.Samples.UWP.Helpers;
using LinqToVso.Samples.UWP.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LinqToVso.Samples.UWP.Services
{
    public enum PageKey
    {
        LoginPage,
        MainPage,
        ProjectPage,
        TeamPage,
        TeamMemberPage,
        ProcessPage
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
            this._pages = new Dictionary<PageKey, Type>
            {
                {PageKey.LoginPage, typeof (LoginPage)},
                {PageKey.MainPage, typeof (MainPage)},
                {PageKey.ProjectPage, typeof (ProjectPage)},
                {PageKey.TeamPage, typeof (TeamPage)},
                {PageKey.ProcessPage, typeof (ProcessPage)},
                {PageKey.TeamMemberPage, typeof (TeamMemberPage)}
            };

            this.CurrentFrame = (Frame)Window.Current.Content;

            var backButton = SystemNavigationManager.GetForCurrentView();
            backButton.BackRequested += (s, e) =>
            {
                e.Handled = true;
                this.GoBack();
            };
        }

        private Frame CurrentFrame { get; }

        public void GoBack()
        {
            if (this.CanGoBack)
            {
                this.CurrentFrame.GoBack();
                this.UpdateBackButtonVisibility();
            }
        }

        public bool CanGoBack => this.CurrentFrame.CanGoBack;

        public void NavigateTo(PageKey page)
        {
            this.NavigateTo(page, null);
        }

        public void NavigateTo(PageKey page, object parameters)
        {
            if (!this._pages.ContainsKey(page))
            {
                throw new ArgumentOutOfRangeException(
                    $"The target page '{page}' does not exist. Did you have initialized the page dictionary correcty?");
            }

            var pageType = this._pages[page];
            this.NavigateToPage(pageType, parameters);

            this.UpdateBackButtonVisibility();
        }

        public void ClearNavigationStack()
        {
            try
            {
                this.CurrentFrame.BackStack.Clear();
                this.UpdateBackButtonVisibility();
            }
            catch (Exception ex)
            {
                // Catch the error
            }
        }

        private void NavigateToPage(Type page, object parameter = null)
        {
            this.CurrentFrame.Navigate(page, parameter);
        }

        private void UpdateBackButtonVisibility()
        {
            if (this.CurrentFrame.CanGoBack)
            {
                TitleBarHelper.ShowBackButton();
            }
            else
            {
                TitleBarHelper.HideBackButton();
            }
        }
    }
}