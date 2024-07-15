using App.ViewModels;
using App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            CurrentItem = loginPageShellContent;

            // Initial check if user is superuser and show/hide the SuperUserFlyoutItem
            CheckSuperUser();
        }

        public async void CheckSuperUser()
        {
            var isSuperUser = await SecureStorage.GetAsync("is_superuser");
            if (!string.IsNullOrEmpty(isSuperUser) && bool.TryParse(isSuperUser, out bool isSuperUserBool) && isSuperUserBool)
            {
                SuperUserFlyoutItem1.IsVisible = true;
            }
            else
            {
                SuperUserFlyoutItem1.IsVisible = false;
            }
        }


        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
