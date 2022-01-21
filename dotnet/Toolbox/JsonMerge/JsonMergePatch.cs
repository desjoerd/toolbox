using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Toolbox.JsonMerge
{
    public static class JsonMergePatch
    {
        /// <summary>
        /// Creates a new object with the json merge patch applied to the source object
        /// </summary>
        /// <typeparam name="TPatched">Type of source</typeparam>
        /// <param name="baseValue">The object which is the starting point for the merge</param>
        /// <param name="patch"></param>
        /// <param name="options"></param>
        /// <returns>A new object with the changes of the json merge patch applied</returns>
        public static TPatched? MergePatch<TPatched>(TPatched baseValue, JsonElement patch, JsonSerializerOptions? options = null)
        { 
            var sourceNode = JsonSerializer.SerializeToNode(baseValue, options);
            var result = MergeCore(sourceNode, patch);

            return result.Deserialize<TPatched>(options);
        }

        /// <summary>
        /// Creates a new object with the json merge patch applied to the source object
        /// </summary>
        /// <typeparam name="TBase">Type of source</typeparam>
        /// <typeparam name="TPatched">Type of the merged result</typeparam>
        /// <param name="baseValue">The object which is the starting point for the merge</param>
        /// <param name="patch"></param>
        /// <param name="options"></param>
        /// <returns>A new object with the changes of the json merge patch applied</returns>
        public static TPatched? MergePatch<TBase, TPatched>(TBase baseValue, JsonElement patch, JsonSerializerOptions? options = null)
        {
            var sourceNode = JsonSerializer.SerializeToNode(baseValue, options);
            var result = MergeCore(sourceNode, patch);

            return result.Deserialize<TPatched>(options);
        }

        private static JsonNode? MergeCore(JsonNode? target, JsonElement patch)
        {
            switch (patch.ValueKind)
            {
                case JsonValueKind.Object:
                {
                    if (target is not JsonObject)
                    {
                        target = new JsonObject(target?.Options);
                    }

                    foreach (var patchProperty in patch.EnumerateObject())
                    {
                        if (patchProperty.Value.ValueKind == JsonValueKind.Null)
                        {
                            ((JsonObject)target).Remove(patchProperty.Name);
                        }
                        else
                        {
                            ((JsonObject)target)[patchProperty.Name] = MergeCore(target[patchProperty.Name], patchProperty.Value);
                        }
                    }

                    return target;
                }
                case JsonValueKind.Array:
                    return JsonArray.Create(patch);
                default:
                    return JsonValue.Create(patch);
            }
        }
    }
}
