using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using TermisWorkerService;
using System.IO; 


    var builder = Host.CreateApplicationBuilder(args);

    builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    builder.Configuration.AddEnvironmentVariables();

    // Configure the database context
    builder.Services.AddDbContext<CsvContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    }, ServiceLifetime.Transient);

    // Configure AppSettings sections
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.Configure<ColumnIndexes>(builder.Configuration.GetSection("ColumnIndexes"));

    // Register the Worker as a singleton service
    builder.Services.AddSingleton<Worker>();
    builder.Services.AddHostedService<Worker>();
    var host = builder.Build();
    host.Run();
