using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Toolbox.JsonMerge
{
    internal class JsonMergePatchConverter<TPatch> : JsonConverter<JsonMergePatchDocument<TPatch>>
    {
        public override JsonMergePatchDocument<TPatch> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var patchElement = JsonElement.ParseValue(ref reader);

            return new JsonMergePatchDocument<TPatch>(patchElement);
        }

        public override void Write(Utf8JsonWriter writer, JsonMergePatchDocument<TPatch> value, JsonSerializerOptions options)
        {
            value.RawPatchJson.WriteTo(writer);
        }
    }
}
