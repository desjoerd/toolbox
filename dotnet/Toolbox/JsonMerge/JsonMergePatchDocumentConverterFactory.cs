using System.Text.Json;
using System.Text.Json.Serialization;

namespace Toolbox.JsonMerge
{
    public class JsonMergePatchDocumentConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert is { IsGenericType: true } && typeToConvert.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>);

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(typeToConvert);

            var patchType = typeToConvert.GetGenericArguments()[0];
            var converterType = typeof(JsonMergePatchConverter<>).MakeGenericType(patchType);

            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
