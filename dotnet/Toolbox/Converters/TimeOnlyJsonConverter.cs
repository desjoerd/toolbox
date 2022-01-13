using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Toolbox.Converters;

/// <summary>
/// Custom <see cref="JsonConverter"/> used to convert to and from <see cref="TimeOnly"/>.
/// It uses partial-time notation as defined by RFC 3339, section 5.6, for example, 21:35:19.123
/// </summary>
public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private const string SerializeTimeFormat = "HH:mm:ss.FFFFFFF";
    private static readonly string[] DeserializeTimeFormats = new[] { SerializeTimeFormat, "HH:mm" };

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (TimeOnly.TryParseExact(reader.GetString(), DeserializeTimeFormats, out var result))
        {
            return result;
        }
        // throw a json exception because a FormatException is not handled as a valid error scenario by ASP.NET Core MVC
        throw new JsonException($"Expected time to be in format 'HH:mm:ss.FFF' as defined in RFC 3339, section 5.6 'partial-time'.");
    }

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Called by System.Text.Json.")]
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(SerializeTimeFormat, CultureInfo.InvariantCulture));
}
