using System.Text.Json;

namespace ScadaMobileMaui;

public partial class TileView : ContentView
{
    public TileView()
    {
        InitializeComponent();
    }

    public void SetData(dynamic obj)
    {
        Dictionary<string, object> kv = JsonSerializer.Deserialize<Dictionary<string, object>>(obj);
        TitleLabel.Text = kv["name"].ToString();
        heatingMeasuredTemperature.Text = kv["heatingMeasuredTemperature"].ToString() + " °C";
        domesticHotWaterMeasuredTemperature.Text = kv["domesticHotWaterMeasuredTemperature"].ToString() + " °C";
        heatmeterEnergy.Text = kv["heatmeterEnergy"].ToString() + " MWh";
        heatmeterDebit.Text = kv["heatmeterDebit"].ToString() + " l/h";
        heatmeterPower.Text = kv["heatmeterPower"].ToString() + " kW";
        DateLabel.Text = DateTime.Parse(kv["measuredDate"].ToString()!).ToString("dd.MM.yyyy HH:mm");
    }
}