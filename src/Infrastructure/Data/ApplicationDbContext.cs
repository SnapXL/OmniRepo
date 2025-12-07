// using OmniRepo.Application.Common.Interfaces;

using OmniRepo.Application.Common.Interfaces;
using OmniRepo.Domain.Entities;
// using OmniRepo.Infrastructure.Identity;

namespace OmniRepo.Infrastructure.Data;


public class ApplicationDbContext : IApplicationDbContext
{
    private readonly List<TodoList> _todoLists = new();
    private readonly List<TodoItem> _todoItems = new();

    public List<TodoList> TodoLists => _todoLists;
    public List<TodoItem> TodoItems => _todoItems;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
