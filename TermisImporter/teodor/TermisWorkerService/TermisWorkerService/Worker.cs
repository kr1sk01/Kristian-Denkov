using Microsoft.Extensions.Options;
using TermisWorkerService.Services;

namespace TermisWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher _fileWatcher = new();
        private readonly string _csvDirectory;

        public Worker(IServiceScopeFactory serviceScopeFactory, ILogger<Worker> logger, IOptions<ServiceSettings> serviceSettings)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _csvDirectory = serviceSettings.Value.CsvDirectory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _fileWatcher = new FileSystemWatcher
            {
                Path = _csvDirectory,
                Filter = "*.csv",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            _fileWatcher.Created += OnCsvFileCreated;
            _fileWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        private void OnCsvFileCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"New CSV file detected: {e.Name}");

            Task.Run(() =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var csvProcessingService = scope.ServiceProvider.GetRequiredService<ICsvService>();
                    try
                    {
                        csvProcessingService.ProcessCsvFile(e.FullPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing CSV files.");
                    }
                }
            });
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}
