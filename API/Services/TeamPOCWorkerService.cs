using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using API.Entities;
using API.Handlers;
using API.Controllers;
using API.Interfaces;
using API.Enums;

namespace API.Services;

public class TeamPOCWorker : IHostedService
{
    private readonly ConcurrentQueue<Job> _jobQueue = JobsController.GetJobQueue();
    private readonly JobHandlerFactory _jobHandlerFactory;
    private readonly ILogger<TeamPOCWorker> _logger;
    private readonly ProcessingMode _processingMode;
    private Timer? _timer;

    public TeamPOCWorker(
        JobHandlerFactory jobHandlerFactory,
        ILogger<TeamPOCWorker> logger,
        IConfiguration configuration)
    {
        _jobHandlerFactory = jobHandlerFactory ?? throw new ArgumentNullException(nameof(jobHandlerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var mode = configuration["ProcessingMode"];
        _processingMode = Enum.TryParse<ProcessingMode>(mode, true, out var parsed) ? parsed : ProcessingMode.Single;
        _logger.LogInformation("Processing mode set to {Mode}", _processingMode);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("TeamPOCWorker is starting.");
        _timer = new Timer(ProcessJobs, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    private async void ProcessJobs(object? state)
    {
        _logger.LogDebug("Checking for jobs in the queue...");

        if (_processingMode == ProcessingMode.Parallel)
        {
            var jobs = new List<Job>();
            while (_jobQueue.TryDequeue(out var job))
                jobs.Add(job);

            var tasks = jobs.Select(job => ProcessJob(job)).ToArray();
            await Task.WhenAll(tasks);
        }
        else // Single
        {
            while (_jobQueue.TryDequeue(out var job))
            {
                await ProcessJob(job);
            }
        }
    }

    private async Task ProcessJob(Job job)
    {
        _logger.LogInformation("Dequeued job {JobId} of type {Type}.", job.ID, job.Type);

        var handler = _jobHandlerFactory.GetHandler(job.Type);
        if (handler != null)
        {
            try
            {
                _logger.LogInformation("Processing job {JobId} with handler {Handler}.", job.ID, handler.GetType().Name);
                await handler.HandleAsync(job);

                _logger.LogInformation("Job {JobId} processed successfully.", job.ID);
            }
            catch (Exception ex)
            {
                job.Status = JobStatus.Failed;
                _logger.LogError(ex, "Error processing job {JobId}.", job.ID);
            }
        }
        else
        {
            job.Status = JobStatus.NoHandler;
            _logger.LogWarning("No handler found for job type {Type}. Job {JobId} will not be processed.", job.Type, job.ID);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("TeamPOCWorker is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}