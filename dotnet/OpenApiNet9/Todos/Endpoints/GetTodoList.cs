using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OpenApiNet9.Todos.Endpoints;

public static class GetTodoList
{
    public static void MapGetTodoList(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("", Handler)
            // this adds metadata (Route and Endpoint) for the name, OpenApi uses IEndpointNameMetadata
            .WithName("GetTodoList")
            // this adds IEndpointDescriptionMetadata
            .WithDescription("This is the openapi description, which is added as IEndpointDescriptionMetadata")
            // this adds IEndpointSummaryMetadata
            .WithSummary("This is the openapi summary, which is added as IEndpointSummaryMetadata")
            .RequireAuthorization()
            .RequireAuthorization("assertion")
            .RequireAuthorization(policy => policy.RequireClaim("claim", "value"));
    }

    public class TodoListItem
    {
        public required string Title { get; set; }

        public bool IsDone { get; set; }
    }

    public static Ok<IEnumerable<TodoListItem>> Handler(
        [FromQuery] string filter = "all")
    {
        return TypedResults.Ok(new[]
        {
            new TodoListItem
            {
                Title = "Todo 1",
                IsDone = false,
            },
            new TodoListItem
            {
                Title = "Done",
                IsDone = true
            }
        }.AsEnumerable());
    }

    // public static Results<Ok<IEnumerable<TodoListItem>>, InternalServerError> Handler()
    // {
    //     return TypedResults.Ok(new[]
    //     {
    //         new TodoListItem
    //         {
    //             Title = "Todo 1",
    //             IsDone = false,
    //         },
    //         new TodoListItem
    //         {
    //             Title = "Done",
    //             IsDone = true
    //         }
    //     }.AsEnumerable());
    // }
}