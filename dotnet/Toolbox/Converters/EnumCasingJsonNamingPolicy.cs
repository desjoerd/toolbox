using System.Text.Json;

namespace Toolbox.Converters;

public class EnumCasingJsonNamingPolicy : JsonNamingPolicy
{
    private readonly char? _wordSeperator;

    public static JsonNamingPolicy CamelCase => JsonNamingPolicy.CamelCase;
    public static EnumCasingJsonNamingPolicy PascalCase { get; }
    public static EnumCasingJsonNamingPolicy SnakeCase { get; }
    public static EnumCasingJsonNamingPolicy UpperSnakeCase { get; }
    public static EnumCasingJsonNamingPolicy KebabCase { get; }

    public EnumCasingJsonNamingPolicy(char? wordSeperator = '_')
    {
        this._wordSeperator = wordSeperator;
    }

    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }
        if (name.Length == 1)
        {
            return name.ToUpperInvariant();
        }
        var nameSpan = name.AsSpan();
        var buffer = new char[name.Length * 2].AsSpan();
        var start = 0;
        var position = 0;
        for (var i = 1; i < nameSpan.Length; i++)
        {
            if (char.IsUpper(name[i]))
            {
                if (start != 0)
                {
                    buffer[position++] = '_';
                }

                position += nameSpan[start..i].ToUpperInvariant(buffer[position..]);
                start = i;
            }
            else if (name[i] == '_')
            {
                // skip current and next
                i += 2;
            }
        }

        if (start != 0)
        {
            buffer[position++] = '_';
        }

        position += nameSpan[start..].ToUpperInvariant(buffer[position..]);

        return new string(buffer[..position]);
    }
}
