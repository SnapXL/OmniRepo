using OmniRepo.Domain.Entities;

namespace OmniRepo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    List<TodoList> TodoLists { get; }

    List<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
