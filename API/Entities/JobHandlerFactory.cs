using System.Collections.Generic;
using API.Interfaces;

namespace API.Entities;

public class JobHandlerFactory
{
    private readonly Dictionary<string, IJobHandler> _handlerMap;

    public JobHandlerFactory(IEnumerable<IJobHandler> jobHandlers)
    {
        _handlerMap = jobHandlers.ToDictionary(handler => handler.CanHandleType, handler => handler);
    }

    public IJobHandler? GetHandler(string jobType)
    {
        _handlerMap.TryGetValue(jobType, out var handler);
        return handler;
    }
}