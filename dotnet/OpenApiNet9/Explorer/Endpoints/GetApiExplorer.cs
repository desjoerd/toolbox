using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using OpenApiNet9.OpenApi;

namespace OpenApiNet9.Explorer.Endpoints;

public static class GetApiExplorer
{
    public static void MapGetApiExplorer(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api-explorer", Handler);
    }

    public static async Task<Ok<List<ApiDescriptionGroupModel>>> Handler(
        [FromServices] IServiceProvider serviceProvider,
        [FromServices] IApiDescriptionGroupCollectionProvider apiExplorer)
    {
        var policyProvider = serviceProvider.GetService<IAuthorizationPolicyProvider>();

        var result = new List<ApiDescriptionGroupModel>();

        foreach (var group in apiExplorer.ApiDescriptionGroups.Items)
        {
            var items = new List<ApiDescriptionModel>();
            foreach (var description in group.Items)
            {
                items.Add(await ApiDescriptionModel.Create(policyProvider, description));
            }

            result.Add(new ApiDescriptionGroupModel(group.GroupName, items));
        }

        return TypedResults.Ok(result);
    }

    public class ApiDescriptionGroupModel(string? groupName, IReadOnlyList<ApiDescriptionModel> items)
    {
        public string? GroupName { get; set; } = groupName;
        public IReadOnlyList<ApiDescriptionModel> Items { get; set; } = items;
    }

    public class ApiDescriptionModel
    {
        private ApiDescriptionModel()
        {
        }

        public string? HttpMethod { get; set; }
        public string? RelativePath { get; set; }
        public required IEnumerable<MetadataModel> Metadata { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AuthorizationDescriptor? Authorization { get; set; }

        public static async Task<ApiDescriptionModel> Create(IAuthorizationPolicyProvider? policyProvider, ApiDescription description)
        {
            return new ApiDescriptionModel
            {
                HttpMethod = description.HttpMethod,
                RelativePath = description.RelativePath,
                Metadata = description.ActionDescriptor.EndpointMetadata.Select(x => new MetadataModel(x)),
                Authorization = await AuthorizationDescriptor.Create(policyProvider, description),
            };
        }
    }

    public class MetadataModel
    {
        public MetadataModel(object x)
        {
            Name = x.GetType().Name;
            Summary = x.ToString();
            Description = x switch
            {
                IAuthorizeData authorizeData => $"Policy :{authorizeData.Policy}, Roles: {string.Join(", ", authorizeData.Roles)}, AuthenticationSchemes: {string.Join(", ", authorizeData.AuthenticationSchemes)}",
                _ => null
            };
            Interfaces = x.GetType().GetInterfaces().Select(x => x.Name).ToArray();
        }

        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Summary { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Interfaces { get; set; }
    }

    public class AuthorizationDescriptor
    {
        public bool IsAllowAnonymous { get; set; }
        public bool RequiresAuthorization { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<RequirementDescriptor>? Requirements { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string>? AuthenticationSchemes { get; set; }

        public static async Task<AuthorizationDescriptor?> Create(IAuthorizationPolicyProvider? policyProvider, ApiDescription description)
        {
            if (policyProvider is null)
            {
                return null;
            }

            var isAllowAnonymous = description.IsAllowAnonymous();
            var policy = await description.GetAuthorizationPolicy(policyProvider);

            if (policy is null)
            {
                return new AuthorizationDescriptor
                {
                    IsAllowAnonymous = isAllowAnonymous,
                    RequiresAuthorization = false,
                };
            }

            return new AuthorizationDescriptor
            {
                IsAllowAnonymous = isAllowAnonymous,
                RequiresAuthorization = true,
                Requirements = policy.Requirements.Select(requirement => new RequirementDescriptor(requirement)).ToList(),
                AuthenticationSchemes = policy.AuthenticationSchemes,
            };
        }
    }

    public class RequirementDescriptor(IAuthorizationRequirement requirement)
    {
        public string? Name { get; set; } = requirement.GetType().Name;
        public string? Description { get; set; } = requirement.ToString();
    }
}