using App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAttestationPage : ContentPage
    {
        public Item Item { get; set; }

        private List<Frame> dynamicFrames = new List<Frame>();

        public ObservableCollection<AttestationQuestion> AttestationQuestions { get; set; }

        List<Entry> nameList = new List<Entry>();
        List<Picker> typeList = new List<Picker>();
        List<Entry> rightvaluesList = new List<Entry>();
        List<Entry> valuesList = new List<Entry>();

        int localId = 0;

        public EditAttestationPage(int id)
        {
            InitializeComponent();

            LoadDataAsync(id);

            localId = id; 

            AttestationQuestions = new ObservableCollection<AttestationQuestion>
            {
            };

        }

        private async void Add_Attestation(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage());
        }

        private string content;

        private async Task LoadDataAsync(int id)
        {
            try
            {
                HttpClient _client = new HttpClient();
                HttpResponseMessage response = await _client.GetAsync($"{GlobalSettings.ApiUrl}/api/attestations/{id}/");
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    // Parse JSON response
                    var todoItem = Newtonsoft.Json.JsonConvert.DeserializeObject<Attestation>(content);

                    // Set the text of the label with the data from the response
                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (todoItem?.AttestationQuestions != null)
                        {
                            titleEntry.Text = todoItem.Name;
                            foreach (var question in todoItem.AttestationQuestions)
                            {
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
                                    Text = question.Name,
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
                                int entryIndex = picker.ItemsSource.IndexOf(question.Type);
                                picker.SelectedIndex = entryIndex;
                                stackLayout.Children.Add(picker);
                                Entry rightValuesEntry = new Entry
                                {
                                    Placeholder = "Правильное значание(я)",
                                    Text = question.RightValues,
                                    FontSize = 20,
                                    FontFamily = "Geist",
                                    AutomationId = $"rightvalues_{(dynamicFrames.Count + 1)}",
                                };
                                stackLayout.Children.Add(rightValuesEntry);
                                Entry valuesEntry = new Entry
                                {
                                    Placeholder = "Значения (для CheckBox и RadioButton)",
                                    Text = question.Values,
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
                        }
                        else
                        {
                            Console.WriteLine("No attestation questions found.");
                        }
                    });
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
            // Get the last dynamically added frame
            if (dynamicFrames.Count > 0)
            {
                // Get the last dynamically added frame
                Frame lastFrame = dynamicFrames[dynamicFrames.Count - 1];

                // Remove the frame from the UI
                FrameContainer.Children.Remove(lastFrame);

                // Remove the frame from the list
                dynamicFrames.Remove(lastFrame);

                // Check if there are any items left in the list
                if (nameList.Count > 0)
                {
                    // Remove the last item from the name list
                    nameList.RemoveAt(nameList.Count - 1);
                }
            }
        }
        private async void Save_Attestation(object sender, EventArgs e)
        {
            AttestationQuestions.Clear();

            for (int i = 0; i < nameList.Count; i++)
            {
                AttestationQuestions.Add(new AttestationQuestion { Id = dynamicFrames.Count + 1, Name = nameList[i].Text, Type = typeList[i].SelectedItem.ToString(), Values = valuesList[i].Text, RightValues = rightvaluesList[i].Text });
            }

            var jsonObject = new
            {
                Title = titleEntry.Text,
                AttestationQuestions = AttestationQuestions,
            };

            // Serialize the JSON object to a string
            var json = JsonConvert.SerializeObject(jsonObject);

            // Create a StringContent with the JSON string
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Replace "your_api_endpoint" with your actual API endpoint
            try
            {
                // Create an HttpClient instance
                using (var client = new HttpClient())
                {
                    // Send the POST request with the JSON content
                    var response = await client.PutAsync($"{GlobalSettings.ApiUrl}/api/attestations/{localId}/", content);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Ред. аттестации", "Аттестация успешна обновлена", "OK");
                    }
                    else
                    {
                        // Request failed
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

        private async void Delete_Attestation(object sender, EventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"{GlobalSettings.ApiUrl}/api/attestations/{localId}/");

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Ред. аттестации", "Аттестация успешна удалена", "OK");

                        await Shell.Current.GoToAsync($"//{nameof(ItemsPage)}");
                    }
                    else
                    {
                        // Request failed
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

        public class Attestation
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<AttestationQuestion> AttestationQuestions { get; set; }
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