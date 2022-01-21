using System.Text.Json;
using System.Text.Json.Serialization;

namespace Toolbox.JsonMerge;

[JsonConverter(typeof(JsonMergePatchDocumentConverterFactory))]
public sealed class JsonMergePatchDocument<T>
{
    public JsonElement RawPatchJson { get; }

    public JsonMergePatchDocument(JsonElement rawPatchJson)
    {
        RawPatchJson = rawPatchJson;
    }

    public T? CreatePatchedValue(T baseValue, JsonSerializerOptions? options = null)
        => JsonMergePatch.MergePatch(baseValue, RawPatchJson, options);

    public T? CreatePatchedValue<TBase>(TBase baseValue, JsonSerializerOptions? options = null)
        => JsonMergePatch.MergePatch<TBase, T>(baseValue, RawPatchJson, options);
}