using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using Serilog;
using System.ComponentModel.DataAnnotations;
using TermisWorkerService.Helper;
using TermisWorkerService.Services;

namespace TermisWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Initial configuration for Serilog to log startup errors
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateBootstrapLogger();

                Log.Information("Starting up the service");

                var host = CreateHostBuilder(args).Build();

                EnsureDatabaseCreated(host.Services);

                ValidateConfiguration(host.Services);

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occurred");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseWindowsService()
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                var logDirectory = hostContext.Configuration.GetSection("ServiceSettings:LogDirectory").Value;
                loggerConfiguration
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .WriteTo.File($"{logDirectory}/TermisLog.txt", rollingInterval: RollingInterval.Day);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<ServiceSettings>(options =>
                {
                    hostContext.Configuration.GetSection("ServiceSettings").Bind(options);
                    options.CsvDirectory = Helper.PathHelper.CorrectPath(options.CsvDirectory);
                    options.ProcessedDirectory = Helper.PathHelper.CorrectPath(options.ProcessedDirectory);
                    options.ErrorDirectory = Helper.PathHelper.CorrectPath(options.ErrorDirectory);
                    options.LogDirectory = Helper.PathHelper.CorrectPath(options.LogDirectory);
                });

                services.Configure<EmailSettings>(hostContext.Configuration.GetSection("EmailSettings"));
                services.Configure<ColumnIndexSettings>(hostContext.Configuration.GetSection("ColumnIndexSettings"));

                services.AddDbContext<ServiceDbContext>(options =>
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                services.AddTransient<ICsvService, CsvService>();
                services.AddTransient<IEmailService, EmailService>();

                services.AddHostedService<Worker>();
            });

        private static void ValidateConfiguration(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var serviceSettings = serviceProvider.GetRequiredService<IOptions<ServiceSettings>>().Value;
                var emailSettings = serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
                var columnIndexSettings = serviceProvider.GetRequiredService<IOptions<ColumnIndexSettings>>().Value;

                ValidateSettings(serviceSettings);
                ValidateSettings(emailSettings);
                ValidateSettings(columnIndexSettings);
            }
        }

        private static void ValidateSettings<T>(T settings) where T : class
        {
            var context = new ValidationContext(settings, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(settings, context, results, true))
            {
                foreach (var validationResult in results)
                {
                    Log.Fatal($"Configuration error: {validationResult.ErrorMessage}");
                }

                throw new Exception("Configuration validation failed. Please check the appsettings.json file.");
            }
        }

        private static void EnsureDatabaseCreated(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
                context.Database.EnsureCreated();
            }
        }
    }
}