using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OpenApiNet9.Todos.Endpoints;

public static class GetTodoById
{
    public static void MapGetTodoById(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("{id}", Handler)
            .WithName("GetTodoById")
            .WithDescription("This is the openapi description, which is added as IEndpointDescriptionMetadata")
            .WithSummary("This is the openapi summary, which is added as IEndpointSummaryMetadata");
    }

    public static string LinkToGetTodoById(this LinkGenerator linkGenerator, string id)
    {
        return linkGenerator.GetPathByName("GetTodoById", new { id })!;
    }

    public class TodoItem
    {
        public required string Title { get; set; }

        public bool IsDone { get; set; }
    }

    public static Ok<TodoItem> Handler(
        [FromRoute] string id)
    {
        return TypedResults.Ok(new TodoItem
        {
            Title = "Todo 1",
            IsDone = false,
        });
    }
}