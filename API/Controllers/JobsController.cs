using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Entities;
using API.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private static readonly ConcurrentQueue<Job> JobQueue = new();
    private static readonly List<Job> AllJobs = new();
    private readonly IEnumerable<IJobHandler> _jobHandlers;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IEnumerable<IJobHandler> jobHandlers, ILogger<JobsController> logger)

    {
        _jobHandlers = jobHandlers ?? throw new ArgumentNullException(nameof(jobHandlers));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("enqueue")]
    public IActionResult EnqueueJob([FromBody] Job job)
    {
          _logger.LogInformation("Received request to enqueue job. Type: {Type}, Payload: {Payload}", job.Type, job.Payload);

        if (string.IsNullOrEmpty(job.Payload) || string.IsNullOrEmpty(job.Type))
        {
             _logger.LogWarning("Enqueue failed: Payload or Type missing.");
            return BadRequest("Payload and Type are required.");
        }

        // Check if any handler can process the job type
        if (!_jobHandlers.Any(handler => handler.CanHandle(job.Type)))
        {
            _logger.LogWarning("Enqueue failed: No handler found for job type '{Type}'.", job.Type);
            return BadRequest($"No handler found for job type '{job.Type}'.");
        }

        job.ID = Guid.NewGuid();
        job.Status = "Pending";
        job.ProcessedTime = null;

        JobQueue.Enqueue(job);
        AllJobs.Add(job);

        _logger.LogInformation("Job {JobId} enqueued successfully.", job.ID);
        return Ok(new { Message = "Job enqueued successfully.", JobId = job.ID });
    }

    [HttpGet("pending")]
    public IActionResult GetPendingJobs()
    {
        var pendingJobs = AllJobs.Where(job => job.Status == "Pending").ToList();
        _logger.LogInformation("Returning {Count} pending jobs.", pendingJobs.Count);
        return Ok(pendingJobs);
    }

    // Expose the job queue for the worker
    public static ConcurrentQueue<Job> GetJobQueue() => JobQueue;
}