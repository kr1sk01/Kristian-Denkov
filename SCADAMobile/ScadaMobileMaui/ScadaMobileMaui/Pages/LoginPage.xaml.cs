using System.Text;
using System.Text.Json;

namespace ScadaMobileMaui.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
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

        if (jsonResponse.GetProperty("isSuccess").GetBoolean())
        {
            var token = jsonResponse.GetProperty("token").GetString();
            // Navigate to the main page and replace the navigation stack
            Application.Current.MainPage = new NavigationPage(new MainPage(token!));
        }
        else
        {
            var errors = jsonResponse.GetProperty("errors").GetProperty("$values").EnumerateArray()
                .Select(e => e.GetString()).ToArray();
            ErrorLabel.Text = string.Join("\n", errors);
            ErrorLabel.IsVisible = true;
        }
    }
}