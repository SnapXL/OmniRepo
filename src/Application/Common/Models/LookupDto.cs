using OmniRepo.Domain.Entities;

namespace OmniRepo.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public static LookupDto FromTodoList(TodoList todoList)
    {
        return new LookupDto
        {
            Id = todoList.Id,
            Title = todoList.Title
        };
    }

    public static LookupDto FromTodoItem(TodoItem todoItem)
    {
        return new LookupDto
        {
            Id = todoItem.Id,
            Title = todoItem.Title
        };
    }
}
