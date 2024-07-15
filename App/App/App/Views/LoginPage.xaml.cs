using App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System.Net.Http;
using Xamarin.Forms.PlatformConfiguration;
using Plugin.Toast;
using Plugin.Toast.Abstractions;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        Label myLabel;
        Entry myEmail;
        Entry myPassword;
        Button myButtonSignIn;
        Button myButtonSignUp;

        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();

            myLabel = new Label
            {
                Text = "Привет",
                Opacity = 100,
                FontFamily = "Geist",
                FontSize = 60,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            myEmail = new Entry
            {
                Placeholder = "электронная почта",
                Opacity = 100,
                FontFamily = "Geist",
                FontSize = 32,
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center,
            };

            myPassword = new Entry
            {
                Placeholder = "пароль",
                Opacity = 100,
                FontFamily = "Geist",
                FontSize = 32,
                TextColor = Color.Black,
                IsPassword = true,
                VerticalOptions = LayoutOptions.Center
            };

            myButtonSignIn = new Button
            {
                Padding = 20,
                Text = "авторизоваться",
                Opacity = 100,
                FontFamily = "Geist",
                FontSize = 24,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#606c38"),
                CornerRadius = 20,
                VerticalOptions = LayoutOptions.Center,
            };

            myButtonSignUp = new Button
            {
                Padding = 20,
                Text = "зарегистрироваться",
                Opacity = 100,
                FontFamily = "Geist",
                FontSize = 24,
                TextColor = Color.Black,
                BackgroundColor = Color.FromHex("#fefae0"),
                CornerRadius = 20,
                VerticalOptions = LayoutOptions.Center,
            };

            myButtonSignIn.Clicked += MyButtonSignIn_Clicked;
            myButtonSignUp.Clicked += MyButtonSignUp_Clicked;

            // Add new elements to the existing StackLayout
            contentLayout.Children.Add(myLabel);
            contentLayout.Children.Add(myEmail);
            contentLayout.Children.Add(myPassword);
            contentLayout.Children.Add(myButtonSignIn);
            contentLayout.Children.Add(myButtonSignUp);

            var linearGradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
            };

            // Add initial gradient stops
            var startColor = Color.LightYellow;
            var endColor = Color.DeepSkyBlue;
            linearGradientBrush.GradientStops.Add(new GradientStop { Color = startColor, Offset = 0 });
            linearGradientBrush.GradientStops.Add(new GradientStop { Color = endColor, Offset = 1 });

            // Set the gradient brush as the background of the layout
            contentLayout.Background = linearGradientBrush;

            AddGlassOverlay();

            AnimateGradientColors(linearGradientBrush);

            image.Source = ImageSource.FromResource("App.Images.sign_in.png");

        }
        private async void MyButtonSignUp_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
        }


        private async void MyButtonSignIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var jsonObject = new
                {
                    email = myEmail.Text,
                    password = myPassword.Text
                };

                var json = JsonConvert.SerializeObject(jsonObject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync($"{GlobalSettings.ApiUrl}/api/token/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                        if (tokenResponse != null)
                        {
                            await SecureStorage.SetAsync("refresh_token", tokenResponse.refresh);
                            await SecureStorage.SetAsync("access_token", tokenResponse.access);
                            await SecureStorage.SetAsync("is_superuser", tokenResponse.is_superuser);

                            await DisplayAlert("Авторизация", "Авторизация прошла успешна", "OK");

                            myEmail.Text = "";
                            myPassword.Text = "";

                            // Check and update the superuser flyout item visibility
                            (App.Current.MainPage as AppShell)?.CheckSuperUser();

                            await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Неправильно набран логин или пароль", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Неправильно набран логин или пароль", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Неправильно набран логин или пароль", "OK");
            }
        }

        void AddGlassOverlay()
        {
            // Create a semi-transparent BoxView to overlay on top
            var overlay = new BoxView
            {
                Color = Color.FromRgba(255, 255, 255, 100), // Adjust alpha channel for transparency
                HeightRequest = double.MaxValue // Ensure the overlay covers the entire height
            };

            // Add overlay to the layout
            contentLayout.Children.Add(overlay);
        }

        async void AnimateGradientColors(LinearGradientBrush brush)
        {
            while (true)
            {
                await Task.Delay(2000); // Delay before next animation

                // Define new gradient colors
                var startColor = Color.LightYellow;
                var endColor = Color.White;

                // Animate the gradient colors
                await AnimateGradientStopColor(brush.GradientStops[0], startColor, 2000);
                await AnimateGradientStopColor(brush.GradientStops[1], endColor, 2000);

                // Repeat the animation
                await Task.Delay(2000); // Delay before next animation

                // Define new gradient colors
                startColor = Color.AntiqueWhite;
                endColor = Color.NavajoWhite;

                // Animate the gradient colors
                await AnimateGradientStopColor(brush.GradientStops[0], startColor, 2000);
                await AnimateGradientStopColor(brush.GradientStops[1], endColor, 2000);
            }
        }

        async Task AnimateGradientStopColor(GradientStop gradientStop, Color targetColor, uint duration)
        {
            var startTime = DateTime.UtcNow;
            var startColor = gradientStop.Color;

            while ((DateTime.UtcNow - startTime).TotalMilliseconds < duration)
            {
                var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
                var ratio = Math.Min(elapsed / duration, 1);
                var newColor = Lerp(startColor, targetColor, (float)ratio);
                gradientStop.Color = newColor;
                await Task.Delay(16); // Adjust for smoother animation
            }

            gradientStop.Color = targetColor;
        }

        // Linear interpolation between two colors
        public static Color Lerp(Color color1, Color color2, float ratio)
        {
            double r = color1.R + (ratio * (color2.R - color1.R));
            double g = color1.G + (ratio * (color2.G - color1.G));
            double b = color1.B + (ratio * (color2.B - color1.B));
            double a = color1.A + (ratio * (color2.A - color1.A));
            return Color.FromRgba(r, g, b, a);
        }

        public class TokenResponse
        {
            public string refresh { get; set; }
            public string access { get; set; }
            public string is_superuser { get; set; }
        }
    }
}