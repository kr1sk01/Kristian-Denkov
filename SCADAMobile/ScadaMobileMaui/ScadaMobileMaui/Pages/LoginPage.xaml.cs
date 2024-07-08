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

        // Client-side validation for email format
        if (!IsValidEmail(email))
        {
            ErrorLabel.Text = "Invalid email address format";
            ErrorLabel.IsVisible = true;
            return;
        }

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

        //if (jsonResponse.GetProperty("isSuccess").GetBoolean())
        //{
        //    var token = jsonResponse.GetProperty("token").GetString();
        //    // Navigate to the main page and replace the navigation stack
        //    Application.Current.MainPage = new NavigationPage(new MainPage(token!));
        //}
        //else
        //{
        //    var errors = jsonResponse.GetProperty("errors").GetProperty("$values").EnumerateArray()
        //        .Select(e => e.GetString()).ToArray();
        //    ErrorLabel.Text = string.Join("\n", errors);
        //    ErrorLabel.IsVisible = true;
        //}
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Regular expression for validating email format
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}