using System;
using API.Enums;

namespace API.Entities;

public class Job
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string Payload { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public DateTime? StartTime { get; set; } = null;
    public DateTime? EndTime { get; set; } = null;
    public TimeSpan? ProcessedTime
    {
        get
        {
            if (StartTime.HasValue && EndTime.HasValue)
                return EndTime - StartTime;
            return null;
        }
    }
    public JobType Type { get; set; }
}