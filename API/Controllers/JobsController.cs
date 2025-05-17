using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Entities;
using API.Interfaces;
using API.DTOs;
using API.Enums;

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
    public IActionResult EnqueueJob([FromBody] EnqueueJobDto dto)
    {
        _logger.LogInformation("Received request to enqueue job. Type: {Type}, Payload: {Payload}", dto.Type, dto.Payload);

        if (string.IsNullOrEmpty(dto.Payload))
        {
            _logger.LogWarning("Enqueue failed: Payload missing.");
            return BadRequest("Payload is required.");
        }

        if (!_jobHandlers.Any(handler => handler.CanHandleType == dto.Type))
        {
            _logger.LogWarning("Enqueue failed: No handler found for job type '{Type}'.", dto.Type);
            return BadRequest($"No handler found for job type '{dto.Type}'.");
        }

        var job = new Job
        {
            ID = Guid.NewGuid(),
            Payload = dto.Payload,
            Type = dto.Type,
            Status = JobStatus.Pending,
            StartTime = null,
            EndTime = null
        };

        JobQueue.Enqueue(job);
        AllJobs.Add(job);

        _logger.LogInformation("Job {JobId} enqueued successfully.", job.ID);
        return Ok(new { Message = "Job enqueued successfully.", JobId = job.ID });
    }

    [HttpGet("pending")]
    public IActionResult GetPendingJobs()
    {
        var pendingJobs = AllJobs.Where(job => job.Status == JobStatus.Pending).ToList();
        _logger.LogInformation("Returning {Count} pending jobs.", pendingJobs.Count);
        return Ok(pendingJobs);
    }

    [HttpGet("listJobs")]
    public IActionResult GetAllJobs()
    {
        _logger.LogInformation("Returning {Count} All Jobs", AllJobs.Count);
        return Ok(AllJobs);
    }

    public static ConcurrentQueue<Job> GetJobQueue() => JobQueue;
}