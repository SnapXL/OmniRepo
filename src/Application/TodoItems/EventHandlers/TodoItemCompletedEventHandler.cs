using OmniRepo.Domain.Events;
using Microsoft.Extensions.Logging;

namespace OmniRepo.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventHandler
{
    private readonly ILogger<TodoItemCompletedEventHandler> _logger;

    public TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("OmniRepo Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
