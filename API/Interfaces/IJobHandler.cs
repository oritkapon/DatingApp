using System.Threading.Tasks;
using API.Entities;
using API.Enums;

namespace API.Interfaces;

public interface IJobHandler
{
    JobType CanHandleType { get; }
    Task HandleAsync(Job job);
}