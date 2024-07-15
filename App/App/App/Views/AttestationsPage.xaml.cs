using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttestationsPage : ContentPage
    {
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

        public AttestationsPage()
        {
            InitializeComponent();
            //ViewDetailsCommand = new Command<Attestation>(ViewDetails);
            image.Source = ImageSource.FromResource("App.Images.attestations.png");

            Title = "Аттестации";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDataAsync();
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
                    await Navigation.PushAsync(new AboutPage(attestation.Id));
                };

                stackLayout.Children.Add(attestationButton);
            }
        }
    }
}