using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMs }";
        }

        private List<string> PrepData()
        {
            List<string> output = new List<string>();

            resultsWindow.Text = "";
           
            output.Add("https://www.yahoo.com/123123123233123123123");
            output.Add("https://www.yahoo.com/");
            output.Add("https://www.yahoo.com/123123123233123123123");
            output.Add("https://www.yahoo.com/");
            output.Add("https://www.yahoo.com/123123123233123123123");
            output.Add("https://www.yahoo.com/");
            return output;
        }



        private async Task RunDownloadParallelAsync()
        {
            List<string> targetUrl = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();
            
            foreach (string s in targetUrl)
            {

               tasks.Add(DownloadWebsiteAsync(s));       
            }
            var results = await Task.WhenAll(tasks);
            foreach (var task in results)
            {
                ReportWebsiteInfo(task);
            }
        }

        
        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            try//Trying to catch all possible exceptions and log the exception on the WP
            {
                output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);
            }   
            catch(Exception e)
            {
                resultsWindow.Text += "\n" + e.Message;
                return null;
            }
            return output;
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            if(data!=null)
            resultsWindow.Text += $"\n{ data.WebsiteUrl } downloaded: { data.WebsiteData.Length } characters long.";
        }


    }
}
