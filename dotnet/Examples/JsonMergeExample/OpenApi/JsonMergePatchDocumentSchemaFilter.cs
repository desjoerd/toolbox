using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Toolbox.JsonMerge;

namespace JsonMergeExample.OpenApi
{
    public class JsonMergePatchDocumentSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!IsJsonMergePatchDocument(context.Type))
            {
                return;
            }

            var underlyingType = context.Type.GetGenericArguments()[0];

            var underlyingSchema = context.SchemaGenerator.GenerateSchema(
                underlyingType,
                context.SchemaRepository,
                context.MemberInfo,
                context.ParameterInfo
            );

            // map all members of underlyingSchema to schema
            foreach (var member in typeof(OpenApiSchema).GetProperties())
            {
                var value = member.GetValue(underlyingSchema);
                member.SetValue(schema, value);
            }
        }

        private bool IsJsonMergePatchDocument(Type type)
            => type is { IsGenericType: true } && type.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>);
    }
}
