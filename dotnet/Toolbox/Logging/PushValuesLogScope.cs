using System.Collections;
using System.Text;

namespace Toolbox.Logging;

public class PushValuesLogScope : IReadOnlyList<KeyValuePair<string, object?>>
{
    private string? _cachedToString;
    private readonly List<KeyValuePair<string, object?>> _properties;

    public PushValuesLogScope(IEnumerable<KeyValuePair<string, object?>> values)
    {
        // logging should not throw exceptions
        if (values is null)
        {
            _properties = new List<KeyValuePair<string, object?>>(0);
        }
        else
        {
            _properties = new List<KeyValuePair<string, object?>>(values);
        }
    }

    public int Count => _properties.Count;

    public KeyValuePair<string, object?> this[int index] => _properties[index];

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        => _properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_properties).GetEnumerator();

    public override string ToString()
    {
        if (_cachedToString is not null)
        {
            return _cachedToString;
        }
        var sb = new StringBuilder();
        foreach (var property in _properties)
        {
            sb.Append($"{property.Key}:{property.Value}, ");
        }
        if (_properties.Count > 0)
        {
            sb.Remove(sb.Length - ", ".Length, ", ".Length);
        }
        return _cachedToString = sb.ToString();
    }
}
