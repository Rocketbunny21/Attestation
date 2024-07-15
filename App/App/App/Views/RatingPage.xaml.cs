using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Diagnostics;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatingPage : ContentPage
    {
        public RatingPage()
        {
            InitializeComponent();

            Title = "Рейтинг";

            image.Source = ImageSource.FromResource("App.Images.rating.png");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            MainStackLayout.Children.Clear();
            var users = await GetUsersAsync();
            PopulateGrid(users);
        }

        private async Task<List<User>> GetUsersAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"{GlobalSettings.ApiUrl}/api/rating/");
                return JsonConvert.DeserializeObject<List<User>>(response);
            }
        }

        private void PopulateGrid(List<User> users)
        {
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                            ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(100) }
                }
            };

            // Add Headers
            grid.Children.Add(new Label
            {
                Text = "ФИО",
                FontAttributes = FontAttributes.Bold,
                FontSize = 32,
                FontFamily = "Geist",
                TextColor = Color.Black
            }, 0, 0);

            grid.Children.Add(new Label
            {
                Text = "Очков",
                FontAttributes = FontAttributes.Bold,
                FontSize = 32,
                FontFamily = "Geist",
                TextColor = Color.Black
            }, 1, 0);

            // Add divider after header
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(new BoxView
            {
                Color = Color.Black,
                HeightRequest = 1
            }, 0, 1);
            Grid.SetColumnSpan(grid.Children[grid.Children.Count - 1], 2);

            int row = 2; // Start from the row after the header divider
            foreach (var user in users)
            {
                // Add user details
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.Children.Add(new Label
                {
                    Padding = 10,
                    Text = $"{user.Firstname} {user.Lastname}",
                    FontSize = 24,
                    FontFamily = "Geist",
                    TextColor = Color.Black
                }, 0, row);

                grid.Children.Add(new Label
                {
                    Padding = 10,
                    Text = user.Score.ToString(),
                    FontSize = 24,
                    FontFamily = "Geist",
                    TextColor = Color.Black
                }, 1, row);

                row++;

                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.Children.Add(new BoxView
                {
                    Color = Color.Black,
                    HeightRequest = 1
                }, 0, row);

                Grid.SetColumnSpan(grid.Children[grid.Children.Count - 1], 2);
                row++;
            }

            MainStackLayout.Children.Add(grid);

        }

        public class User
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public int Score { get; set; }
        }
    }
}