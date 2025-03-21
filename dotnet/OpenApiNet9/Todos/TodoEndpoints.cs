using OpenApiNet9.Todos.Endpoints;

namespace OpenApiNet9.Todos;

public static class TodoEndpoints
{
    public static void MapTodos(this IEndpointRouteBuilder routes)
    {
        var authorizedGroup = routes.MapGroup("")
            .RequireAuthorization();



        var group = routes.MapGroup("todos")
            .WithTags("bla");

        group.MapGetTodoList();
        group.MapGetTodoById();
        group.MapPostTodo();
    }
}