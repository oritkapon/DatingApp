using API.Entities;

namespace API.Interfaces;

public interface IJobHandler
{
    string CanHandleType { get; }
    bool CanHandle(string jobType);
    Task HandleAsync(Job job);
}