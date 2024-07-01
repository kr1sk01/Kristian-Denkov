using Android.Service.QuickSettings;
using System.Text.Json;

namespace ScadaMobileMaui
{
    public partial class MainPage : ContentPage
    {
        private readonly string _token;

        public MainPage(string token)
        {
            InitializeComponent();
            _token = token;
            LoadTiles();
        }

        private async void LoadTiles()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            var response = await client.GetStringAsync("https://api.toplo.test.stemo.bg/api/Stations/GetStationStandartView");
            var jsonResponse = JsonDocument.Parse(response).RootElement;

            // Get the array of objects inside the "values" property
            var objects = jsonResponse.GetProperty("$values").EnumerateArray();

            foreach (var obj in objects)
            {
                var tile = new TileView();
                tile.SetData(obj);
                TileStack.Children.Add(tile);
            }
        }
    }

}
