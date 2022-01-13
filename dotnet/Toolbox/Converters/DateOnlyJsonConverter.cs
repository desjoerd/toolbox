using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Toolbox.Converters;

/// <summary>
/// Custom <see cref="JsonConverter"/> used to convert to and from <see cref="DateOnly"/>.
/// It uses full-date notation as defined by RFC 3339, section 5.6, for example, 2017-07-21
/// </summary>
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (DateOnly.TryParseExact(reader.GetString(), DateFormat, out var result))
        {
            return result;
        }
        // throw a json exception because a FormatException is not handled as a valid error scenario by ASP.NET Core MVC
        throw new JsonException($"Expected date to be in format 'yyyy-MM-dd' as defined in RFC 3339, section 5.6 'full-date'.");
    }

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Called by System.Text.Json.")]
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
}
