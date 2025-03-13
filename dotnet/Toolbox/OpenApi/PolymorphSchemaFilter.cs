
namespace Toolbox;

using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class PolymorphSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.MemberInfo is not null
            || context.ParameterInfo is not null)
        {
            // This is a property or parameter, not a type.
            return;
        }

        if (context.Type.GetCustomAttributes<JsonDerivedTypeAttribute>().Any())
        {
            ApplyBaseType(schema, context);
        }
        else if (context.Type.GetBaseTypes().SelectMany(x => x.GetCustomAttributes<JsonDerivedTypeAttribute>()).Any())
        {
            ApplyInheritedType(schema, context);
        }
    }

    private static void ApplyBaseType(OpenApiSchema schema, SchemaFilterContext context)
    {
        var oneOf = new List<OpenApiSchema>();
        var jsonDerivedTypes = context.Type
            .GetCustomAttributes<JsonDerivedTypeAttribute>()
            .ToList();

        var discriminatorProperty = GetDiscriminatorPropertyName(context.Type);

        var discriminatorMapping = new Dictionary<string, string>();
        foreach (var jsonDerivedType in jsonDerivedTypes)
        {
            var derivedType = jsonDerivedType.DerivedType;
            var schemaRef = context.SchemaGenerator.GenerateSchema(derivedType, context.SchemaRepository);

            oneOf.Add(schemaRef);
            discriminatorMapping.Add(jsonDerivedType.TypeDiscriminator?.ToString() ?? derivedType.Name, schemaRef.Reference.ReferenceV3);
        }

        schema.Type = "object";
        schema.OneOf = oneOf;

        var discriminatorSchema = new OpenApiSchema
        {
            Type = "string",
            Enum = discriminatorMapping.Keys
                .Select(x => new OpenApiString(x))
                .Cast<IOpenApiAny>()
                .ToList(),
        };

        schema.Properties = new Dictionary<string, OpenApiSchema>()
        {
            { discriminatorProperty, discriminatorSchema },
        };
        schema.Discriminator = new OpenApiDiscriminator
        {
            PropertyName = discriminatorProperty,
            Mapping = discriminatorMapping,
        };
        schema.Required.Add(discriminatorProperty);
    }

    private static void ApplyInheritedType(OpenApiSchema schema, SchemaFilterContext context)
    {
        var discriminatorProperty = GetDiscriminatorPropertyName(context.Type);
        var jsonDerivedTypes = context.Type
            .GetBaseTypes()
            .SelectMany(x => x.GetCustomAttributes<JsonDerivedTypeAttribute>());
        var typeName = jsonDerivedTypes.FirstOrDefault(x => x.DerivedType == context.Type)?.TypeDiscriminator?.ToString() ?? context.Type.Name;

        var sortedProperties = new SortedDictionary<string, OpenApiSchema>(new DiscriminatorFirstComparer(discriminatorProperty));
        foreach (var (key, value) in schema.Properties)
        {
            sortedProperties.Add(key, value);
        }

        schema.Properties = sortedProperties;

        if (!schema.Properties.TryGetValue(discriminatorProperty, out var discriminatorPropertySchema))
        {
            discriminatorPropertySchema = new OpenApiSchema
            {
                Type = "string",
            };
            schema.Properties.Add(discriminatorProperty, discriminatorPropertySchema);
        }

        discriminatorPropertySchema.Enum = new List<IOpenApiAny>
        {
            new OpenApiString(typeName),
        };
    }

    private static string GetDiscriminatorPropertyName(Type type) =>
        type.GetCustomAttributes<JsonPolymorphicAttribute>()
            .Concat(type.GetBaseTypes().SelectMany(x => x.GetCustomAttributes<JsonPolymorphicAttribute>()))
            .Select(x => x.TypeDiscriminatorPropertyName)
            .FirstOrDefault() ?? "$type";

    private class DiscriminatorFirstComparer(string discriminatorProperty) : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == discriminatorProperty)
            {
                return -1;
            }

            if (y == discriminatorProperty)
            {
                return 1;
            }

            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}
