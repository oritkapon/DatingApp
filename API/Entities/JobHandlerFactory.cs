using System.Collections.Generic;
using System.Linq;
using API.Interfaces;
using API.Enums;

namespace API.Entities;

public class JobHandlerFactory
{
    private readonly Dictionary<JobType, IJobHandler> _handlerMap;

    public JobHandlerFactory(IEnumerable<IJobHandler> jobHandlers)
    {
        _handlerMap = jobHandlers.ToDictionary(handler => handler.CanHandleType, handler => handler);
    }

    public IJobHandler? GetHandler(JobType jobType)
    {
        _handlerMap.TryGetValue(jobType, out var handler);
        return handler;
    }
}