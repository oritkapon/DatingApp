using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace API.Services;

    public class LoggingBackgroundService : BackgroundService
    {
        private readonly Serilog.ILogger _logger;

        public LoggingBackgroundService(Serilog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("LoggingBackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.Information("LoggingBackgroundService is running at: {Time}", DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Task was canceled, exit gracefully
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred in LoggingBackgroundService.");
                }
            }

            _logger.Information("LoggingBackgroundService is stopping.");
        }
    }
