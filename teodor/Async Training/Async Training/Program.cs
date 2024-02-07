//This program lets the user input a list of URLs
//Then it asynchronously retrieves information from each one
//And finally prints out the information its retrieved
using System.Net;
    
namespace Async_Training
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> websiteList = GetWebsiteList();
            Task.Run(() => RunDownloadAsync(websiteList));
            Console.ReadLine();
        }

        static List<string> GetWebsiteList()
        {
            List<string> websiteList = new List<string>();

            string? userInput;

            //Loop for handling user input. Typing "exit" ends the loop
            while (true) 
            {
                Console.WriteLine("Enter website URL: ");
                userInput = Console.ReadLine();
                if (userInput == null || userInput == "")
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }
                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    if (websiteList.Count == 0)
                    {
                        Console.WriteLine("You haven't entered any websites yet.");
                        continue;
                    }
                    break;
                }
                websiteList.Add(userInput);
            }

            return websiteList;
        }

        static async Task RunDownloadAsync(List<string> websites)
        {
            //Creating a list of tasks so we can download website information in parallel
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            //Downloading the information from each website
            foreach (string website in websites)
            {
                tasks.Add(DownloadWebsiteAsync(website));
            }

            //Collect the information from all the websites once they're all done
            var results = await Task.WhenAll(tasks);

            //Print out each website's information
            foreach (var item in results)
            {
                PrintWebsiteInfo(item);
            }
        }

        static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel website = new WebsiteDataModel();
            HttpClient client = new HttpClient();

            //Trying to connect to specified website to retrieve its information
            try
            {
                website.WebsiteUrl = websiteURL;
                website.WebsiteData = await client.GetStringAsync(websiteURL);
            }
            catch (Exception e)
            {
                website.WebsiteData = $"Error downloading {websiteURL}: {e.Message}";
            }

            return website;
        }

        static void PrintWebsiteInfo(WebsiteDataModel data)
        {
            //Printing out each website's information
            if(data.WebsiteData.StartsWith("Error downloading"))
                Console.WriteLine(data.WebsiteData);           
            else
                Console.WriteLine($"{data.WebsiteUrl} downloaded: {data.WebsiteData.Length} characters long.");
        }
    }
}
