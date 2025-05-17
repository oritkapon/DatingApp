using System;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;

namespace API.Handlers;

public class TypeBHandler : IJobHandler

{
    public string CanHandleType => "B";
    public async Task HandleAsync(Job job)
    {
        // Simulate processing
        await Task.Delay(1000);
        job.Status = "Processed by TypeBHandler";
        job.ProcessedTime = DateTime.UtcNow;
        Console.WriteLine($"Job {job.ID} processed by TypeBHandler at {job.ProcessedTime}");
    }

    public bool CanHandle(string jobType)
    {
        return jobType == "B";
    }
}