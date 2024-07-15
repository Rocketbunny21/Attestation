using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static App.Views.AboutPage;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            //LoadData();

            Title = "Настройки";

            image.Source = ImageSource.FromResource("App.Images.settings.png");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Call your data loading method here
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                HttpClient client = new HttpClient();
                var access_token = await SecureStorage.GetAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                HttpResponseMessage response = await client.GetAsync($"{GlobalSettings.ApiUrl}/api/get-user/");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var todoItem = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(content);

                    Email.Text = todoItem.Email;
                    FirstName.Text = todoItem.Firstname;
                    LastName.Text = todoItem.Lastname;
                }
                else
                {
                    // Handle unsuccessful response
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }

        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            try
            {
                var jsonObject = new
                {
                    FirstName = FirstName.Text,
                    LastName = LastName.Text,
                    Email = Email.Text,
                    Password = Password.Text,
                };

                var json = JsonConvert.SerializeObject(jsonObject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    var access_token = await SecureStorage.GetAsync("access_token");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                    HttpResponseMessage response = await client.PostAsync($"{GlobalSettings.ApiUrl}/api/user/change/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Настройки", "Сохранение прошло успешно", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", $"Ошибка получения данных. Статус код: {response.StatusCode}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"{ex.Message}", "OK");
            }
        }
        public class User
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
        }
    }
}