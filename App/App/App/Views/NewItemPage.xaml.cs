using App.Models;
using App.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static App.Views.NewItemPage;

namespace App.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        private List<Frame> dynamicFrames = new List<Frame>();
        public ObservableCollection<AttestationQuestion> AttestationQuestions { get; set; }

        List<Entry> nameList = new List<Entry>();
        List<Picker> typeList = new List<Picker>();
        List<Entry> rightvaluesList = new List<Entry>();
        List<Entry> valuesList = new List<Entry>();

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();

            Title = "Добавление аттестации";

            AttestationQuestions = new ObservableCollection<AttestationQuestion>{};
        }

        private void AddFrame_Clicked(object sender, EventArgs e)
        {
            // Create a new frame
            Frame newFrame = new Frame
            {
                CornerRadius = 20,
                BorderColor = Color.Black,
                BackgroundColor = Color.White,
                Margin = new Thickness(0, 10, 0, 0) // Optional margin to separate frames
            };

            // Add content to the new frame
            StackLayout stackLayout = new StackLayout();
            Entry nameEntry = new Entry
            {
                Placeholder = "Текст вопроса",
                FontSize = 20,
                FontFamily = "Geist",
                AutomationId = $"name_{(dynamicFrames.Count + 1)}",
            };
            stackLayout.Children.Add(nameEntry);
            Picker picker = new Picker
            {
                Title = "Выберите тип",
                FontSize = 20,
                FontFamily = "Geist",
                ItemsSource = new List<string>
                {
                    "Entry",
                    "Checkbox",
                    "RadioButton",
                },
                AutomationId = $"picker_{(dynamicFrames.Count + 1)}",
            };
            stackLayout.Children.Add(picker);
            Entry rightValuesEntry = new Entry
            {
                Placeholder = "Правильное значание(я)",
                FontSize = 20,
                FontFamily = "Geist",
                AutomationId = $"rightvalues_{(dynamicFrames.Count + 1)}",
            };
            stackLayout.Children.Add(rightValuesEntry);
            Entry valuesEntry = new Entry
            {
                Placeholder = "Значения (для CheckBox и RadioButton)",
                FontSize = 20,
                FontFamily = "Geist",
                AutomationId = $"values_{(dynamicFrames.Count + 1)}",
            };
            stackLayout.Children.Add(valuesEntry);
            
            newFrame.Content = stackLayout;

            nameList.Add(nameEntry);
            typeList.Add(picker);
            rightvaluesList.Add(rightValuesEntry);
            valuesList.Add(valuesEntry);

            FrameContainer.Children.Add(newFrame);

            dynamicFrames.Add(newFrame);

        }

        private void RemoveFrame_Clicked(object sender, EventArgs e)
        {
            if (dynamicFrames.Count > 0)
            {
                Frame lastFrame = dynamicFrames[dynamicFrames.Count - 1];
                FrameContainer.Children.Remove(lastFrame);
                dynamicFrames.Remove(lastFrame);
                if (nameList.Count > 0)
                {
                    nameList.RemoveAt(nameList.Count - 1);
                }
            }
        }
        private async void Save_Attestation(object sender, EventArgs e)
        {
            AttestationQuestions.Clear();

            for (int i = 0; i < nameList.Count; i++)
            {
                AttestationQuestions.Add(new AttestationQuestion { Id = dynamicFrames.Count + 1, Name = nameList[i].Text, 
                    Type = typeList[i].SelectedItem.ToString(), Values = valuesList[i].Text, RightValues = rightvaluesList[i].Text });
            }

            var jsonObject = new
            {
                Title = titleEntry.Text,
                AttestationQuestions = AttestationQuestions,
            };

 
            var json = JsonConvert.SerializeObject(jsonObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync($"{GlobalSettings.ApiUrl}/api/create/attestation/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Добавление аттестации", "Аттестация успешна добавлена", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось отправить запрос.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // Exception occurred
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async void Cancel_Attestation(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(ItemsPage)}");
        }

        public class AttestationQuestion
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Values { get; set; }
            public string RightValues { get; set; }
        }

    }
}