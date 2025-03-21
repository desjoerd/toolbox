using OpenApiNet9.Explorer.Endpoints;

namespace OpenApiNet9.Explorer;

public static class ExplorerEndpoints
{
    public static void MapExplorer(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("explorer");

        group.MapGetApiExplorer();
    }
}