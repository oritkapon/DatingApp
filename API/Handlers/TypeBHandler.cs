using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using API.Enums;
using Microsoft.Extensions.Logging;

namespace API.Handlers;

public class TypeBHandler : IJobHandler
{
    private readonly ILogger<TypeBHandler> _logger;

    public TypeBHandler(ILogger<TypeBHandler> logger)
    {
        _logger = logger;
    }

    public JobType CanHandleType => JobType.B;

    public async Task HandleAsync(Job job)
    {
        _logger.LogInformation("Starting processing job {JobId} of type B.", job.ID);
        job.Status = JobStatus.Executing;
        job.StartTime = DateTime.UtcNow;
        try
        {
            await Task.Delay(10000); // Simulate work
            job.EndTime = DateTime.UtcNow;
            job.Status = JobStatus.Completed;
            _logger.LogInformation("Completed processing job {JobId}. Duration: {Duration} ms.", job.ID, (job.EndTime - job.StartTime)?.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.EndTime = DateTime.UtcNow;
            _logger.LogError(ex, "Error processing job {JobId}.", job.ID);
        }
    }
}