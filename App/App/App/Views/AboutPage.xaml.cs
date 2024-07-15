using Microcharts;
using Microcharts.Forms;
using SkiaSharp;
using System;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using static App.Views.AboutPage;
using System.Linq;
using System.Net.Http.Headers;
using Xamarin.Essentials;
using System.Text;
using System.Collections;

namespace App.Views
{
    public partial class AboutPage : ContentPage
    {
        private int score = 0;

        private readonly HttpClient _client = new HttpClient();

        public AboutPage(int id)
        {
            InitializeComponent();
            LoadData(id);

            image.Source = ImageSource.FromResource("App.Images.test_logo.jpg");
        }

        List<Entry> entryList = new List<Entry>();
        Dictionary<string, List<string>> questionSelections = new Dictionary<string, List<string>>();

        private string content;

        private async void LoadData(int id)
        {

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{GlobalSettings.ApiUrl}/api/attestations/{id}/");
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    // Parse JSON response
                    var todoItem = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(content);

                    // Set the text of the label with the data from the response
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        title.Text = todoItem?.Name; // Assuming 'label' is the name of your label control

                        //attestationQuestions.Add(todoItem.AttestationQuestions);

                        if (todoItem?.AttestationQuestions != null)
                        {
                            foreach (var question in todoItem.AttestationQuestions)
                            {
                                if (question.Type == "Entry")
                                {
                                    var nameLabel = new Label
                                    {
                                        Text = question.Name,
                                        FontSize = 24,
                                        FontFamily = "Geist",
                                        TextColor = Color.Black,
                                    };
                                    var entry = new Entry
                                    {
                                        AutomationId = question.Id.ToString(),
                                        Placeholder = "Введите ответ",
                                        FontSize = 24,
                                        FontFamily = "Geist",
                                        TextColor = Color.Black,
                                    };
                                    dynamicLayout.Children.Add(nameLabel);
                                    dynamicLayout.Children.Add(entry);

                                    entryList.Add(entry);
                                }
                                else if (question.Type == "Checkbox")
                                {
                                    // Create CheckBox elements with Labels
                                    var valuesArray = question.Values.Split(',');
                                    var checkBoxNameLabel = new Label
                                    {
                                        Text = question.Name,
                                        FontSize = 24,
                                        FontFamily = "Geist",
                                        TextColor = Color.Black,
                                    };
                                    dynamicLayout.Children.Add(checkBoxNameLabel);

                                    foreach (var value in valuesArray)
                                    {
                                        var checkBox = new CheckBox();
                                        checkBox.CheckedChanged += (sender, e) =>
                                        {
                                            CheckBox_CheckedChanged(sender, e, question.Id.ToString(), value.Trim());
                                        };

                                        var stack = new StackLayout
                                        {
                                            Orientation = StackOrientation.Horizontal,
                                            Children =
                                            {
                                                checkBox,
                                                new Label
                                                {
                                                    AutomationId = question.Id.ToString(),
                                                    Text = value.Trim(),
                                                    FontSize = 24,
                                                    FontFamily = "Geist",
                                                    TextColor = Color.Black,
                                                }
                                            }
                                        };
                                        dynamicLayout.Children.Add(stack);
                                    }
                                }
                                else if (question.Type == "RadioButton")
                                {
                                    // Create Radio Button elements with Labels
                                    var valuesArray = question.Values.Split(',');
                                    var radioGroupName = "radioGroup" + question.Id; // Unique group name for each question
                                    var radioGroupNameHash = radioGroupName.GetHashCode(); // Hash the group name for uniqueness

                                    var radioGroupNameLabel = new Label
                                    {
                                        Text = question.Name,
                                        FontSize = 24,
                                        FontFamily = "Geist",
                                        TextColor = Color.Black,
                                    };
                                    dynamicLayout.Children.Add(radioGroupNameLabel);

                                    foreach (var value in valuesArray)
                                    {
                                        var radioButton = new RadioButton { GroupName = radioGroupName };
                                        radioButton.CheckedChanged += (sender, e) =>
                                        {
                                            RadioButton_CheckedChanged(sender, e, question.Id.ToString(), value.Trim(), radioGroupNameHash);
                                        };

                                        var stack = new StackLayout
                                        {
                                            Orientation = StackOrientation.Horizontal,
                                            Children =
                                            {
                                                radioButton,
                                                new Label
                                                {
                                                    AutomationId = question.Id.ToString(),
                                                    Text = value.Trim(),
                                                    FontSize = 24,
                                                    FontFamily = "Geist",
                                                    TextColor = Color.Black,
                                                }
                                            }
                                        };
                                        dynamicLayout.Children.Add(stack);
                                    }
                                }
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

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e, string questionId, string value)
        {
            // Handle checkbox state change here
            var isChecked = e.Value;

            if (!questionSelections.ContainsKey(questionId))
            {
                questionSelections[questionId] = new List<string>();
            }

            if (isChecked)
            {
                // Add the selected option to the respective question's selection list
                questionSelections[questionId].Add(value);
            }
            else
            {
                // Remove the option from the respective question's selection list if it was unchecked
                questionSelections[questionId].Remove(value);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var attestationQuestions = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(content).AttestationQuestions;

            int score = 0; // Initialize the general score

            foreach (Entry entry in entryList)
            {
                var question = attestationQuestions.FirstOrDefault(q => q.Id == int.Parse(entry.AutomationId));

                if (question.Type == "Entry")
                {
                    if (entry.Text == question.RightValues)
                    {
                        score += 1;
                    }
                }
            }

            foreach (var question in attestationQuestions)
            {
                if (question.Type == "Checkbox")
                {
                    // Get the selected options for this question
                    List<string> selectedOptions = questionSelections.ContainsKey(question.Id.ToString()) ?
                                                    questionSelections[question.Id.ToString()] :
                                                    new List<string>();

                    // Split the RightValues string into an array of correct values
                    var rightValuesArray = question.RightValues.Split(',');

                    // Check if the sets of selected options and right values are equal
                    if (selectedOptions.Count == rightValuesArray.Length &&
                        selectedOptions.All(option => rightValuesArray.Contains(option.Trim())))
                    {
                        score += 1; // Increment the general score
                    }
                }
                else if (question.Type == "RadioButton")
                {
                    // Get the selected option for this question
                    string selectedOption = questionSelections.ContainsKey(question.Id.ToString()) ?
                                            questionSelections[question.Id.ToString()].FirstOrDefault() :
                                            null;

                    // Check if the selected option matches any of the correct values
                    if (selectedOption != null && selectedOption == question.RightValues)
                    {
                        score += 1; // Increment the score if the selected option is correct
                    }
                }
            }

            // Calculate the total number of questions
            int totalQuestions = attestationQuestions.Count(q => q.Type == "Entry" || q.Type == "Checkbox" || q.Type == "RadioButton");


            // Calculate the score for the chart
            var categorizedEntries = new[]
            {
                new ChartEntry(score)
                {
                    Label = "Верно",
                    ValueLabel = score.ToString(),
                    Color = SKColor.Parse("#77d065")
                },
                new ChartEntry(totalQuestions - score)
                {
                    Label = "Неверно",
                    ValueLabel = (totalQuestions - score).ToString(),
                    Color = SKColor.Parse("#b455b6")
                }
            };

            // Update the chart with the categorized entries
            chartView.Chart = new DonutChart
            {
                Entries = categorizedEntries
            };

            try
            {
                // Create HttpClient instance
                using (HttpClient client = new HttpClient())
                {

                    // Create an object to send
                    var dataToSend = new
                    {
                        Score = score,
                        // Add more properties as needed
                    };

                    // Serialize object to JSON
                    var json = JsonConvert.SerializeObject(dataToSend);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var access_token = await SecureStorage.GetAsync("access_token");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync($"{GlobalSettings.ApiUrl}/api/attestation-done/", content);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }

            await DisplayAlert("Аттестация", "Аттестация завершена. Вы набрали баллов:"+ score, "OK");
            score = 0;
            await Shell.Current.GoToAsync($"//{nameof(AttestationsPage)}");
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e, string questionId, string selectedValue, int radioGroupNameHash)
        {
            var radioButton = (RadioButton)sender;

            // Ensure that the event is triggered by checking, not unchecking
            if (e.Value)
            {
                // Get the selected option for this question
                string selectedOption = selectedValue.Trim();

                // Update the question selections dictionary with the selected option
                questionSelections[questionId] = new List<string> { selectedOption };

                // Uncheck other radio buttons in the same group
                foreach (var child in dynamicLayout.Children)
                {
                    if (child is StackLayout stackLayout)
                    {
                        foreach (var view in stackLayout.Children)
                        {
                            if (view is RadioButton radio && radio.GroupName.GetHashCode() == radioGroupNameHash && radio != radioButton)
                            {
                                radio.IsChecked = false;
                            }
                        }
                    }
                }
            }
        }


        public class User
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