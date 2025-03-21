using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OpenApiNet9.Todos.Endpoints;

public static class PostTodo
{
    public static void MapPostTodo(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("", Handler)
            .WithName("PostTodo")
            .WithDescription("This is the openapi description, which is added as IEndpointDescriptionMetadata")
            .WithSummary("This is the openapi summary, which is added as IEndpointSummaryMetadata")
            .RequireAuthorization(c => c.RequireRole("admin"))
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes("oauth2")
                .AddRequirements(new AuthScopeRequirement { Scope = "Bla" }));
    }

    public class CreateTodo
    {
        public required string Title { get; set; }
    }

    public static Created Handler(
        [FromServices] LinkGenerator linkGenerator,
        [FromBody] CreateTodo todo)
    {
        return TypedResults.Created(linkGenerator.LinkToGetTodoById(Guid.NewGuid().ToString()));
    }
}