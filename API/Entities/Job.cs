using System;

namespace API.Entities;

public class Job
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string Payload { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? ProcessedTime { get; set; }
    public string Type { get; set; } = string.Empty;
}