using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using API.Entities;
using API.Handlers;
using API.Controllers; 
using API.Interfaces;

namespace API.Services;

public class TeamPOCWorker : IHostedService
{
    private readonly ConcurrentQueue<Job> _jobQueue = JobsController.GetJobQueue();
    private readonly JobHandlerFactory _jobHandlerFactory;
    private Timer? _timer;

    public TeamPOCWorker(JobHandlerFactory jobHandlerFactory)
    {
        _jobHandlerFactory = jobHandlerFactory ?? throw new ArgumentNullException(nameof(jobHandlerFactory));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("TeamPOCWorker is starting.");
        _timer = new Timer(ProcessJobs, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    private async void ProcessJobs(object? state)
    {
        while (_jobQueue.TryDequeue(out var job))
        {
            Console.WriteLine($"Processing job {job.ID} of type {job.Type}");
            
            var handler = _jobHandlerFactory.GetHandler(job.Type);
            if (handler != null)
            {
                await handler.HandleAsync(job);
            }
            else
            {
                Console.WriteLine($"No handler found for job type {job.Type}");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("TeamPOCWorker is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}