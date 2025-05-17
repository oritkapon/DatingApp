using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
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

    public JobsController(IEnumerable<IJobHandler> jobHandlers)
    {
        _jobHandlers = jobHandlers ?? throw new ArgumentNullException(nameof(jobHandlers));
    }

    [HttpPost("enqueue")]
    public IActionResult EnqueueJob([FromBody] Job job)
    {
        if (string.IsNullOrEmpty(job.Payload) || string.IsNullOrEmpty(job.Type))
        {
            return BadRequest("Payload and Type are required.");
        }

        // Check if any handler can process the job type
        if (!_jobHandlers.Any(handler => handler.CanHandle(job.Type)))
        {
            return BadRequest($"No handler found for job type '{job.Type}'.");
        }

        job.ID = Guid.NewGuid();
        job.Status = "Pending";
        job.ProcessedTime = null;

        JobQueue.Enqueue(job);
        AllJobs.Add(job);

        return Ok(new { Message = "Job enqueued successfully.", JobId = job.ID });
    }

    [HttpGet("pending")]
    public IActionResult GetPendingJobs()
    {
        var pendingJobs = AllJobs.Where(job => job.Status == "Pending").ToList();
        return Ok(pendingJobs);
    }

    // Expose the job queue for the worker
    public static ConcurrentQueue<Job> GetJobQueue() => JobQueue;
}