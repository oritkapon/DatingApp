using System;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;

namespace API.Handlers;

public class TypeAHandler : IJobHandler
{
    public string CanHandleType => "A";
    public async Task HandleAsync(Job job)
    {
        // Simulate processing
        await Task.Delay(1000);
        job.Status = "Processed by TypeAHandler";
        job.ProcessedTime = DateTime.UtcNow;
        Console.WriteLine($"Job {job.ID} processed by TypeAHandler at {job.ProcessedTime}");
    }
    public bool CanHandle(string jobType)
    {
        return jobType == "A";
    }
}