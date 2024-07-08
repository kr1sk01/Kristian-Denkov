using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ScadaMobileMaui.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        var email = Email.Text;
        var password = Password.Text;

        var client = new HttpClient();
        var response = await client.PostAsync("https://api.toplo.test.stemo.bg/Api/Auth/Login", new StringContent(
            JsonSerializer.Serialize(new { email, password }), Encoding.UTF8, "application/json"));

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent).RootElement;

        if (response.IsSuccessStatusCode)
        {
            var token = jsonResponse.GetProperty("token").GetString();
            // Navigate to the main page and replace the navigation stack
            Application.Current.MainPage = new NavigationPage(new MainPage(token!));
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            ErrorLabel.Text = "An error occurred during login";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            var errors = jsonResponse.GetProperty("errors");
            string errorValues = string.Empty;

            if (errors.TryGetProperty("$values", out JsonElement values))
            {
                errorValues = string.Join("\n", values.EnumerateArray().Select(e => e.GetString()));
            }
            else
            {
                foreach (JsonProperty property in errors.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        errorValues += string.Join("\n", property.Value.EnumerateArray().Select(e => e.GetString())) + "\n";
                    }
                }
            }

            ErrorLabel.Text = errorValues;
            ErrorLabel.IsVisible = true;
        }
    }
}