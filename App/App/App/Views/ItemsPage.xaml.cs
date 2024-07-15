using App.Models;
using App.ViewModels;
using App.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static App.Views.AttestationsPage;
using static App.Views.ItemsPage;

namespace App.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;
        public class AttestationQuestion
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Values { get; set; }
            public string RightValues { get; set; }
        }

        public class Attestation
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<AttestationQuestion> AttestationQuestions { get; set; }
        }

        public ItemsPage()
        {
            InitializeComponent();
            Title = "Создание аттестаций";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDataAsync();
        }

        private async void Add_Attestation(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage());
        }

        private async Task LoadDataAsync()
        {
            string apiUrl = $"{GlobalSettings.ApiUrl}/api/attestations/";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(apiUrl);
                    List<Attestation> attestations = JsonConvert.DeserializeObject<List<Attestation>>(json);
                    DisplayData(attestations);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Не удалось загрузить данные", "OK");
                }
            }
        }

        private async void DisplayData(List<Attestation> attestations)
        {
            stackLayout.Children.Clear();

            foreach (var attestation in attestations)
            {
                // Create a label for the attestation name
                Button attestationButton = new Button
                {
                    Text = attestation.Name,
                    FontSize = 20,
                    CornerRadius = 60,
                    FontFamily = "Geist",
                    TextColor = Color.White,
                    BackgroundColor = Color.FromHex("#606c38"),
                };

                // Optional: Set a click event for each button
                attestationButton.Clicked += async (sender, args) =>
                {
                    await Navigation.PushAsync(new EditAttestationPage(attestation.Id));
                };

                stackLayout.Children.Add(attestationButton);
            }
        }

        
    }
}