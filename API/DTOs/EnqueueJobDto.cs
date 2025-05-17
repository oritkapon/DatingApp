using System.ComponentModel.DataAnnotations;
using API.Enums;

namespace API.DTOs;

public class EnqueueJobDto
{
    [Required]
    public string Payload { get; set; } = string.Empty;
    [Required]
    public JobType Type { get; set; }
}