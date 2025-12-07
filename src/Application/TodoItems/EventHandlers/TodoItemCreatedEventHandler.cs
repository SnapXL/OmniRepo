using OmniRepo.Domain.Events;
using Microsoft.Extensions.Logging;

namespace OmniRepo.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler
{
    private readonly ILogger<TodoItemCreatedEventHandler> _logger;

    public TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("OmniRepo Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
