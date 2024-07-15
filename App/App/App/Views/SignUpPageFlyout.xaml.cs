using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPageFlyout : ContentPage
    {
        public ListView ListView;

        public SignUpPageFlyout()
        {
            InitializeComponent();

            BindingContext = new SignUpPageFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        private class SignUpPageFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<SignUpPageFlyoutMenuItem> MenuItems { get; set; }

            public SignUpPageFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<SignUpPageFlyoutMenuItem>(new[]
                {
                    new SignUpPageFlyoutMenuItem { Id = 0, Title = "Page 1" },
                    new SignUpPageFlyoutMenuItem { Id = 1, Title = "Page 2" },
                    new SignUpPageFlyoutMenuItem { Id = 2, Title = "Page 3" },
                    new SignUpPageFlyoutMenuItem { Id = 3, Title = "Page 4" },
                    new SignUpPageFlyoutMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}